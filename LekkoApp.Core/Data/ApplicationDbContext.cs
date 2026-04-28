using LekkoApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LekkoApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PomodoroTask> PomodoroTasks { get; set; }
        public DbSet<PomodoroSession> Pomodoros { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PomodoroTask>()
                .HasMany(t => t.Tags)
                .WithMany(tag => tag.PomodoroTasks)
                .UsingEntity(j => j.ToTable("PomodoroTaskTag")
                    .HasOne(typeof(PomodoroTask))
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction));

            modelBuilder.Entity<PomodoroTask>()
                .Property(x => x.TaskNumber)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            modelBuilder.Entity<PomodoroTask>()
                .HasIndex(t => t.IsDeleted);

            modelBuilder.Entity<PomodoroTask>()
                .HasIndex(t => new { t.UserId, t.Status, t.DueDate });

            modelBuilder.Entity<PomodoroTask>()
                .HasIndex(t => t.CreatedAt);

            modelBuilder.Entity<Tag>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}