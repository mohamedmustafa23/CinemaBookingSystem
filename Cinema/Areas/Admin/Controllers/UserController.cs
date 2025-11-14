using Cinema.Models;
using Cinema.Utilities;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public UserController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault() ?? "No Role";
            }

            return View(users);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new List<string>()
            {
                SD.Role_SuperAdmin,
                SD.Role_Admin,
                SD.Role_Customer
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserVM userVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string>()
                {
                    SD.Role_SuperAdmin,
                    SD.Role_Admin,
                    SD.Role_Customer
                };
                return View(userVM);
            }

            var existingUser = await _userManager.FindByEmailAsync(userVM.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");

                ViewBag.Roles = new List<string>()
                {
                    SD.Role_SuperAdmin,
                    SD.Role_Admin,
                    SD.Role_Customer
                };
                return View(userVM);
            }

            var user = new ApplicationUser()
            {
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                UserName = userVM.UserName,
                Email = userVM.Email,
            };

            var result = await _userManager.CreateAsync(user, userVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ViewBag.Roles = new List<string>()
                {
                    SD.Role_SuperAdmin,
                    SD.Role_Admin,
                    SD.Role_Customer
                };
                return View(userVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account",
                new { area = "Identity", userId = user.Id, token },
                Request.Scheme, Request.Host.ToString());

            await _emailSender.SendEmailAsync(userVM.Email, "Confirm your email",
                $"<h1>Confirm your email by Clicking <a href='{link}'>Here</a></h1>");

            await _userManager.AddToRoleAsync(user, userVM.Role);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult LockUnLock(string? id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }
            var user = _userManager.FindByIdAsync(id).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound();
            }
            if (_userManager.IsInRoleAsync(user, SD.Role_SuperAdmin).GetAwaiter().GetResult())
            {
                TempData["Error"] = "You cannot lock/unlock a Super Admin user.";
                return RedirectToAction("Index");
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
            {
                //user is locked and we need to unlock them
                user.LockoutEnd = DateTimeOffset.Now;
                TempData["Notification"] = "User unlocked successfully.";
            }
            else
            {
                user.LockoutEnd = DateTimeOffset.Now.AddDays(10);
                TempData["Notification"] = "User locked successfully.";
            }
            _userManager.UpdateAsync(user).GetAwaiter().GetResult();
            return RedirectToAction("Index");
        }
    }
}
