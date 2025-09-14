using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Models
{
    public class UrlClick
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UrlId { get; set; }

        [ForeignKey(nameof(UrlId))]
        public Url? Url { get; set; } 
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
        public string? Referrer { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; } 
    }
}
