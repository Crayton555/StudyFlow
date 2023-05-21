using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models.Identity;
using StudyFlow.Models.Domain;
using StudyFlow.Models.Domain.Enumeration;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudyFlow.Data
{
    public class ApplicationDbContext : IdentityDbContext<StudyFlowUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Models.Domain.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.Domain.Task>()
                .Property(x => x.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Models.Domain.Task>()
                .Property(t => t.Priority)
                .HasConversion<string>();

            builder.Entity<Models.Domain.Task>()
                .Property(t => t.Status)
                .HasConversion<string>();

            builder.Entity<StudyFlowUser>()
                .HasMany(c => c.Tasks)
                .WithOne(e => e.User);

            builder.Entity<Models.Domain.Task>()
                .HasOne(e => e.User)
                .WithMany(c => c.Tasks);
        }
    }
}