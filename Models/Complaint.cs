using System.ComponentModel.DataAnnotations;

namespace KabirHostelCare.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Wing { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ProblemTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Submitted";

        public string? WardenRemarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedResolutionDate { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}