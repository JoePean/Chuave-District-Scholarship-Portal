
using Chuave.Scholarship.Api.Data;
using Chuave.Scholarship.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chuave.Scholarship.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ApplicationsController(AppDbContext db) { _db = db; }

        [HttpPost("apply/{scholarshipId:int}")]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> Apply(int scholarshipId)
        {
            var cid = User.Claims.FirstOrDefault(c => c.Type == "ApplicantId")?.Value;
            if (string.IsNullOrEmpty(cid)) return Forbid();
            var applicantId = int.Parse(cid);

            var exists = await _db.Scholarships.AnyAsync(s => s.ScholarshipId == scholarshipId);
            if (!exists) return NotFound("Scholarship not found");

            var already = await _db.Applications.AnyAsync(a => a.ApplicantId == applicantId && a.ScholarshipId == scholarshipId);
            if (already) return BadRequest("Already applied");

            var app = new ApplicationModel { ApplicantId = applicantId, ScholarshipId = scholarshipId, Status = "Pending" };
            _db.Applications.Add(app); await _db.SaveChangesAsync();
            return Ok(app);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> Mine()
        {
            var cid = User.Claims.FirstOrDefault(c => c.Type == "ApplicantId")?.Value;
            if (string.IsNullOrEmpty(cid)) return Forbid();
            var applicantId = int.Parse(cid);

            var list = await _db.Applications.Include(a => a.Scholarship)
                                             .Where(a => a.ApplicantId == applicantId)
                                             .ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var list = await _db.Applications.Include(a => a.Applicant).Include(a => a.Scholarship).ToListAsync();
            return Ok(list);
        }

        [HttpPut("{id:int}/status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetStatus(int id, string status)
        {
            var allowed = new[] { "Pending", "Approved", "Rejected" };
            if (!allowed.Contains(status)) return BadRequest("Invalid status");

            var app = await _db.Applications.FindAsync(id);
            if (app is null) return NotFound();
            app.Status = status; await _db.SaveChangesAsync();
            return Ok(app);
        }
    }
}
