using LekkoApp.Models;
using Microsoft.EntityFrameworkCore;
using Task = LekkoApp.Models.Task;

namespace LekkoApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<PomodoroSession> Pomodoros { get; set; }
        public DbSet<TimerLog> TimerLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .Property(x => x.TaskNumber)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);;
        }
        
    }
}