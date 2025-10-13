
using Chuave.Scholarship.Api.Data;
using Chuave.Scholarship.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chuave.Scholarship.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ApplicantsController(AppDbContext db) { _db = db; }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() => Ok(await _db.Applicants.AsNoTracking().ToListAsync());

        [HttpGet("me")]
        [Authorize(Roles = "Applicant,Admin")]
        public async Task<IActionResult> Me()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "ApplicantId")?.Value;
            if (string.IsNullOrEmpty(claim)) return Forbid();
            var id = int.Parse(claim);
            var a = await _db.Applicants.FindAsync(id);
            return a is null ? NotFound() : Ok(a);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Applicant a)
        {
            _db.Applicants.Add(a); await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = a.ApplicantId }, a);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _db.Applicants.FindAsync(id);
            return a is null ? NotFound() : Ok(a);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Applicant upd)
        {
            var a = await _db.Applicants.FindAsync(id);
            if (a is null) return NotFound();
            _db.Entry(a).CurrentValues.SetValues(upd); await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Applicants.FindAsync(id);
            if (a is null) return NotFound();
            _db.Applicants.Remove(a); await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
