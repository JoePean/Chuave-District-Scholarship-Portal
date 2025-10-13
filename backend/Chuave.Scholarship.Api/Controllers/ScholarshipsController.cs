
using Chuave.Scholarship.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarshipModel = Chuave.Scholarship.Api.Models.Scholarship;

namespace Chuave.Scholarship.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScholarshipsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ScholarshipsController(AppDbContext db) { _db = db; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Scholarships.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ScholarshipModel s)
        {
            _db.Scholarships.Add(s);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = s.ScholarshipId }, s);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _db.Scholarships.FindAsync(id);
            if (s == null) return NotFound();
            return Ok(s);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ScholarshipModel updated)
        {
            var s = await _db.Scholarships.FindAsync(id);
            if (s == null) return NotFound();
            _db.Entry(s).CurrentValues.SetValues(updated);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Scholarships.FindAsync(id);
            if (s == null) return NotFound();
            _db.Scholarships.Remove(s);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
