using System.ComponentModel.DataAnnotations;

namespace KabirHostelCare.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? RoomNumber { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(50)]
        public string? Wing { get; set; }
    }
}