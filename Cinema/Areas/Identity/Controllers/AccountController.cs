using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }

            // Send Confirmation Mail Here
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme, Request.Host.ToString());

            await _emailSender.SendEmailAsync(registerVM.Email, "Confirm your email", $"<h1>Confirm your email by Clicking <a href='{link}'>Here</a></h1>");

            TempData["JustRegistered"] = true;
            return RedirectToAction(nameof(Login));

        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"The User ID {userId} is invalid");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Invalid Email Confirmation Token";
                return RedirectToAction(nameof(Login));
            }
            TempData["Notification"] = "Email Confirmed Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email Or Password.");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
                else if (!user.EmailConfirmed)
                    ModelState.AddModelError(string.Empty, "Please Confirm your Email First!");
                else
                    ModelState.AddModelError(string.Empty, "Invalid User Name / Email Or Password.");

                return View(loginVM);
            }

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmail)
        {
            if (!ModelState.IsValid)
                return View(resendEmail);

            var user = await _userManager.FindByNameAsync(resendEmail.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(resendEmail.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email Or Password.");
                return View(resendEmail);
            }

            if(user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email is already confirmed.");
                return View(resendEmail);
            }

            // Send Confirmation Mail Here
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme, Request.Host.ToString());

            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", $"<h1>Confirm your email by Clicking <a href='{link}'>Here</a></h1>");

            TempData["JustRegistered"] = true;
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email.");
                return View(forgetPasswordVM);
            }

            

            var userOTPs = await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id);
            var totalOTPs = userOTPs.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs >= 3)
            {
                TempData["ErrorMessage"] = "You have exceeded the maximum number of OTP requests. Please try again after 24 hours.";
                return View(forgetPasswordVM);
            }

           
            var otp = new Random().Next(100000, 999999).ToString();

            await _applicationUserOTPRepository.AddAsync(new ApplicationUserOTP
            {
                ApplicationUserId = user.Id,
                CreateAt = DateTime.UtcNow,
                IsValid = true,
                OTP = otp,
                ValidTo = DateTime.UtcNow.AddMinutes(10) 
            });
            await _applicationUserOTPRepository.CommitAsync();

            await _emailSender.SendEmailAsync(user.Email!, "Reset Password - OTP Code",
                $@"<div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2 style='color: #FF4B2B;'>Password Reset Request</h2>
            <p>Your OTP code is:</p>
            <h1 style='color: #FF4B2B; letter-spacing: 5px;'>{otp}</h1>
            <p style='color: #666;'>This code will expire in 10 minutes.</p>
            <p style='color: #666;'>If you didn't request this, please ignore this email.</p>
        </div>");

            TempData["SuccessMessage"] = "OTP code has been sent to your email!";
            TempData["UserEmail"] = user.Email; // ✅ عشان نستخدمه في صفحة الـ OTP
            return RedirectToAction(nameof(ValidateOTP), new { userId = user.Id });
        }

        [HttpGet]
        public IActionResult ValidateOTP(string userId)
        {
            var userEmail = TempData["UserEmail"]?.ToString() ?? "your email";
            return View(new ValidateOTPVM
            {
                ApplicationUserId = userId,
                Email = userEmail 
            });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please enter the complete OTP code.";
                return View(validateOTPVM);
            }

            var result = await _applicationUserOTPRepository.GetOneAsync(
                e => e.ApplicationUserId == validateOTPVM.ApplicationUserId
                && e.OTP == validateOTPVM.OTP
                && e.IsValid
                && e.ValidTo > DateTime.UtcNow); 

            if (result is null)
            {
                TempData["ErrorMessage"] = "Invalid or expired OTP code. Please try again.";
                return View(validateOTPVM);
            }

            
            result.IsValid = false;
            _applicationUserOTPRepository.Update(result);
            await _applicationUserOTPRepository.CommitAsync();

            TempData["SuccessMessage"] = "OTP verified successfully!";
            return RedirectToAction(nameof(NewPassword), new { userId = validateOTPVM.ApplicationUserId });
        }

        [HttpGet]
        public IActionResult NewPassword(string userId)
        {
            return View(new NewPasswordVM
            {
                ApplicationUserId = userId
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(newPasswordVM);

            var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);

            if (user is null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Login));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(newPasswordVM);
            }

            TempData["SuccessMessage"] = "Password reset successfully! You can now sign in.";
            return RedirectToAction(nameof(Login));
        }
    }
}
