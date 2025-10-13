
using Chuave.Scholarship.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chuave.Scholarship.Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Applicant> Applicants { get; set; } = default!;
        public DbSet<Chuave.Scholarship.Api.Models.Scholarship> Scholarships { get; set; } = default!;
        public DbSet<ApplicationModel> Applications { get; set; } = default!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Applicant>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(100).IsRequired();
                e.Property(p => p.Email).HasMaxLength(120);
            });

            b.Entity<Chuave.Scholarship.Api.Models.Scholarship>(e =>
            {
                e.Property(p => p.ScholarshipName).HasMaxLength(100).IsRequired();
                e.Property(p => p.Description).HasMaxLength(500);
                e.Property(p => p.EligibilityCriteria).HasMaxLength(500);
                e.Property(p => p.Sponsor).HasMaxLength(120);
            });

            b.Entity<ApplicationModel>(e =>
            {
                e.Property(p => p.Status).HasMaxLength(30).HasDefaultValue("Pending");
                e.HasOne(p => p.Applicant)
                    .WithMany(a => a.Applications)
                    .HasForeignKey(p => p.ApplicantId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(p => p.Scholarship)
                    .WithMany(s => s.Applications)
                    .HasForeignKey(p => p.ScholarshipId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
