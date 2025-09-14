namespace Exam.Models
{
    public class UrlCreateRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = false;
    }
}
