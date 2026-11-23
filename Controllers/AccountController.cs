using KabirHostelCare.Data;
using KabirHostelCare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KabirHostelCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
public IActionResult Login()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(
    string Email, string Password)
{
    if (string.IsNullOrWhiteSpace(Email) ||
        string.IsNullOrWhiteSpace(Password))
    {
        ViewBag.Error = "Please enter email and password.";
        return View();
    }

    var user = await _userManager.FindByEmailAsync(Email.Trim());

    if (user == null ||
        !await _userManager.IsInRoleAsync(user, "Student"))
    {
        ViewBag.Error = "Invalid email or password.";
        return View();
    }

    var result = await _signInManager.PasswordSignInAsync(
        user.UserName!,
        Password,
        isPersistent: false,
        lockoutOnFailure: false);

    if (result.Succeeded)
    {
        return RedirectToAction("Index", "Student");
    }

    ViewBag.Error = "Invalid email or password.";
    return View();
}

[HttpGet]
public IActionResult AdminLogin()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AdminLogin(string Email, string Password)
{
    if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
    {
        ViewBag.Error = "Email and password are required.";
        return View();
    }

    var user = await _userManager.FindByEmailAsync(Email.Trim());

    if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
    {
        ViewBag.Error = "Invalid warden credentials.";
        return View();
    }

    var result = await _signInManager.PasswordSignInAsync(
        user.UserName!, Password, false, false);

    if (result.Succeeded)
    {
        return RedirectToAction("Index", "Admin");
    }

    ViewBag.Error = "Invalid warden credentials.";
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync();
    return RedirectToAction("Login");
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string FullName, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            if (Password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters.";
                return View();
            }

            var existingUser = await _userManager.FindByEmailAsync(Email);

            if (existingUser != null)
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            var user = new IdentityUser
            {
                UserName = Email.Trim(),
                Email = Email.Trim()
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                var profile = new StudentProfile
                {
                    UserId = user.Id,
                    FullName = FullName.Trim()
                };

                _context.StudentProfiles.Add(profile);
                await _context.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "Student");

                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Student");
            }

            ViewBag.Error = string.Join(" ",
                result.Errors.Select(e => e.Description));

            return View();
        }
    }
}