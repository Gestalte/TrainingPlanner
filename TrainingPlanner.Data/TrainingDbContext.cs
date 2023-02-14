using Microsoft.EntityFrameworkCore;

namespace TrainingPlanner.Data
{
    public class TrainingDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=training.db");
        }

        public DbSet<Schedule> Schedules { get; set; }
    }

    public class Schedule
    {
        public int ScheduleId { get; set; }
        public string Title { get; set; }
        public Int16 Weekday { get; set; }
        public Int16 Timeslot { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
        public bool IsComplete { get; set; }
    }

    public class Exercise
    {
        public int ExerciseId { get; set; }
        public string Description { get; set; }
    }
}
