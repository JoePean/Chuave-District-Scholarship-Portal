
namespace Chuave.Scholarship.Api.Models
{
    public class Applicant
    {
        public int ApplicantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? District { get; set; }
        public string? Province { get; set; }
        public string? School { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public ICollection<ApplicationModel> Applications { get; set; } = new List<ApplicationModel>();
    }
}
