
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chuave.Scholarship.Api.Auth;
using Chuave.Scholarship.Api.Data;
using Chuave.Scholarship.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Chuave.Scholarship.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public AuthController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, IConfiguration config, AppDbContext db)
        {
            _users = users; _signIn = signIn; _config = config; _db = db;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var u = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var result = await _users.CreateAsync(u, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            var role = dto.Role == "Admin" ? "Admin" : "Applicant";
            await _users.AddToRoleAsync(u, role);

            if (role == "Applicant")
            {
                var a = new Applicant { Name = dto.Name ?? dto.Email, Email = dto.Email };
                _db.Applicants.Add(a);
                await _db.SaveChangesAsync();
                u.ApplicantId = a.ApplicantId;
                await _users.UpdateAsync(u);
            }

            return Ok(new { message = "Registered", role });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");
            var ok = await _signIn.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!ok.Succeeded) return Unauthorized("Invalid credentials");

            var roles = await _users.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Applicant";
            var token = Generate(user, role);
            return Ok(token);
        }

        private TokenResponse Generate(ApplicationUser user, string role)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? user.UserName ?? "user"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role),
            };
            if (user.ApplicantId.HasValue) claims.Add(new Claim("ApplicantId", user.ApplicantId.Value.ToString()));

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!));
            var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: expires, signingCredentials: creds);
            return new TokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(token), ExpiresAt = expires, Role = role };
        }
    }
}
