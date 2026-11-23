using KabirHostelCare.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KabirHostelCare.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var profile = await _context.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            var complaints = await _context.Complaints
                .Where(c => c.StudentUserId == user.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.StudentName = profile?.FullName ?? user.Email;
            ViewBag.TotalComplaints = complaints.Count;
            ViewBag.PendingComplaints = complaints.Count(c =>
                c.Status == "Submitted" || c.Status == "Under Review");
            ViewBag.InProgressComplaints = complaints.Count(c =>
                c.Status == "In Progress");
            ViewBag.ResolvedComplaints = complaints.Count(c =>
                c.Status == "Resolved");

            return View(complaints);
        }
    }
}