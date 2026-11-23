using KabirHostelCare.Data;
using KabirHostelCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KabirHostelCare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var complaints = await _context.Complaints
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.TotalComplaints = complaints.Count;
            ViewBag.PendingComplaints = complaints.Count(c =>
                c.Status == "Submitted" ||
                c.Status == "Under Review");
            ViewBag.InProgressComplaints = complaints.Count(c =>
                c.Status == "In Progress");
            ViewBag.ResolvedComplaints = complaints.Count(c =>
                c.Status == "Resolved");

            return View(complaints);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int id)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(
            int id,
            string Status,
            string? WardenRemarks,
            DateTime? ExpectedResolutionDate)
        {
            var allowedStatuses = new[]
            {
                "Submitted",
                "Under Review",
                "In Progress",
                "Resolved"
            };

            if (!allowedStatuses.Contains(Status))
                return BadRequest("Invalid complaint status.");

            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            complaint.Status = Status;
            complaint.WardenRemarks = WardenRemarks;
            complaint.ExpectedResolutionDate = ExpectedResolutionDate;
            complaint.ResolvedAt = Status == "Resolved"
                ? DateTime.UtcNow
                : null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}