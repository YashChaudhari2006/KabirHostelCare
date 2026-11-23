using KabirHostelCare.Data;
using KabirHostelCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KabirHostelCare.Controllers
{
    [Authorize(Roles = "Student")]
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ComplaintController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var profile = _context.StudentProfiles
                .FirstOrDefault(p => p.UserId == user.Id);

            ViewBag.StudentName = profile?.FullName ?? user.Email;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Complaint complaint)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            ModelState.Remove(nameof(Complaint.StudentUserId));
            ModelState.Remove(nameof(Complaint.StudentName));
            ModelState.Remove(nameof(Complaint.Status));
            ModelState.Remove(nameof(Complaint.CreatedAt));
            ModelState.Remove(nameof(Complaint.WardenRemarks));
            ModelState.Remove(nameof(Complaint.ExpectedResolutionDate));
            ModelState.Remove(nameof(Complaint.ResolvedAt));

            if (!ModelState.IsValid)
            {
                var profile = _context.StudentProfiles
                    .FirstOrDefault(p => p.UserId == user.Id);

                ViewBag.StudentName = profile?.FullName ?? user.Email;
                return View(complaint);
            }

            var studentProfile = _context.StudentProfiles
                .FirstOrDefault(p => p.UserId == user.Id);

            complaint.StudentUserId = user.Id;
            complaint.StudentName = studentProfile?.FullName ?? user.Email ?? "Student";
            complaint.Status = "Submitted";
            complaint.CreatedAt = DateTime.UtcNow;

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Student");
        }
    }
}