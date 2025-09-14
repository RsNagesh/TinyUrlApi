using System;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class Url
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public bool IsPrivate { get; set; }
        public int Clicks { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
