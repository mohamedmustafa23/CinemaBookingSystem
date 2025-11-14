using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<BookingSeat> _bookingSeatRepository;
        private readonly IRepository<ShowTime> _showTimeRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(
            IRepository<Booking> bookingRepository,
            IRepository<BookingSeat> bookingSeatRepository,
            IRepository<ShowTime> showTimeRepository,
            IRepository<Movie> movieRepository,
            UserManager<ApplicationUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _bookingSeatRepository = bookingSeatRepository;
            _showTimeRepository = showTimeRepository;
            _movieRepository = movieRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] BookingConfirmVM model)
        {
            try
            {
                // Get current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Validate ShowTime
                var showTime = await _showTimeRepository.GetOneAsync(
                    expression: st => st.Id == model.ShowTimeId,
                    includes: [st => st.Movie , st => st.CinemaHall],
                    tracked: false);

                if (showTime == null)
                {
                    return Json(new { success = false, message = "Show time not found" });
                }

                // Check if show time is still available
                if (showTime.Status != "Active")
                {
                    return Json(new { success = false, message = "Show time is no longer available" });
                }

                // Check if seats are available (you can add more logic here to check actual seat availability)
                var existingBookings = await _bookingSeatRepository.GetAsync(
                    expression: bs => bs.Booking.ShowTimeId == model.ShowTimeId &&
                                  model.SelectedSeats.Contains(bs.SeatNumber) &&
                                  bs.SeatStatus == "Booked");

                if (existingBookings.Any())
                {
                    var occupiedSeats = existingBookings.Select(bs => bs.SeatNumber).ToList();
                    return Json(new
                    {
                        success = false,
                        message = $"The following seats are already booked: {string.Join(", ", occupiedSeats)}"
                    });
                }

                // Calculate total amount
                decimal totalAmount = model.SelectedSeats.Count * showTime.TicketPrice;

                // Generate booking reference
                string bookingReference = GenerateBookingReference();

                // Create booking
                var booking = new Booking
                {
                    UserId = userId,
                    ShowTimeId = model.ShowTimeId,
                    BookingDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    BookingStatus = "Confirmed",
                    BookingReference = bookingReference,
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = "Paid",
                    PaymentDate = DateTime.Now,
                    NumberOfSeats = model.SelectedSeats.Count,
                    Notes = model.Notes
                };

                var createdBooking = await _bookingRepository.AddAsync(booking);
                await _bookingRepository.CommitAsync();

                // Create booking seats
                foreach (var seatNumber in model.SelectedSeats)
                {
                    var bookingSeat = new BookingSeat
                    {
                        BookingId = createdBooking.Id,
                        SeatNumber = seatNumber,
                        Price = showTime.TicketPrice,
                        SeatStatus = "Booked",
                        CreatedDate = DateTime.Now
                    };

                    await _bookingSeatRepository.AddAsync(bookingSeat);
                }

                await _bookingSeatRepository.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Booking confirmed successfully",
                    bookingId = createdBooking.Id,
                    bookingReference = bookingReference,
                    data = new
                    {
                        movieTitle = showTime.Movie?.Title,
                        cinemaHall = showTime.CinemaHall?.Name,
                        showDate = showTime.ShowDate.ToString("dd MMM yyyy"),
                        showTime = showTime.StartTime.ToString(@"hh\:mm"),
                        seats = string.Join(", ", model.SelectedSeats),
                        totalAmount = totalAmount,
                        bookingReference = bookingReference
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = await _bookingRepository.GetAsync(
                expression: b => b.UserId == userId,
                includes:
                [
                    b => b.ShowTime,
                    b => b.ShowTime.Movie,
                    b => b.ShowTime.CinemaHall,
                    b => b.BookingSeats
                ],
                orderBy: query => ((IQueryable<Booking>)query).OrderByDescending(b => b.BookingDate),
                tracked: false);

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> BookingDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = await _bookingRepository.GetOneAsync(
                expression: b => b.Id == id && b.UserId == userId,
                includes:
                [
                    b => b.ShowTime,
                    b => b.ShowTime.Movie,
                    b => b.ShowTime.CinemaHall,
                    b => b.BookingSeats
                ],
                tracked: false);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found";
                return RedirectToAction(nameof(MyBookings));
            }

            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var booking = await _bookingRepository.GetOneAsync(
                    expression: b => b.Id == id && b.UserId == userId,
                    includes:
                    [
                        b => b.ShowTime,
                        b => b.BookingSeats
                    ]);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Booking not found" });
                }

                // Check if booking can be cancelled (e.g., at least 2 hours before show time)
                var showDateTime = booking.ShowTime.ShowDate.Date + booking.ShowTime.StartTime;
                if (showDateTime <= DateTime.Now.AddHours(2))
                {
                    return Json(new { success = false, message = "Bookings cannot be cancelled less than 2 hours before the show" });
                }

                // Update booking status
                booking.BookingStatus = "Cancelled";
                booking.PaymentStatus = "Refunded";

                // Update seat status
                foreach (var seat in booking.BookingSeats)
                {
                    seat.SeatStatus = "Cancelled";
                }

                _bookingRepository.Update(booking);
                await _bookingRepository.CommitAsync();

                return Json(new { success = true, message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckSeatAvailability(int showTimeId, List<string> seats)
        {
            var occupiedSeats = await _bookingSeatRepository.GetAsync(
                expression: bs => bs.Booking.ShowTimeId == showTimeId &&
                              seats.Contains(bs.SeatNumber) &&
                              bs.SeatStatus == "Booked",
                tracked: false);

            var unavailableSeats = occupiedSeats.Select(bs => bs.SeatNumber).ToList();

            return Json(new
            {
                available = !unavailableSeats.Any(),
                unavailableSeats = unavailableSeats
            });
        }

        private string GenerateBookingReference()
        {
            // Generate unique booking reference
            return $"BK{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }
    }
}