
namespace Chuave.Scholarship.Api.Models
{
    public class ApplicationModel
    {
        public int ApplicationModelId { get; set; }
        public int ApplicantId { get; set; }
        public int ScholarshipId { get; set; }
        public string Status { get; set; } = "Pending";

        public Applicant? Applicant { get; set; }
        public Scholarship? Scholarship { get; set; }
    }
}
