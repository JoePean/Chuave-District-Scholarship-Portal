
using Microsoft.AspNetCore.Identity;

namespace Chuave.Scholarship.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? ApplicantId { get; set; }
    }
}
