
namespace Chuave.Scholarship.Api.Models
{
    public class Scholarship
    {
        public int ScholarshipId { get; set; }
        public string ScholarshipName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EligibilityCriteria { get; set; }
        public decimal FundingAmount { get; set; }
        public string? Sponsor { get; set; }
        public DateTime? Deadline { get; set; }

        public ICollection<ApplicationModel> Applications { get; set; } = new List<ApplicationModel>();
    }
}
