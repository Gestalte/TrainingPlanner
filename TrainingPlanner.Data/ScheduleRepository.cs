using Microsoft.EntityFrameworkCore;

namespace TrainingPlanner.Data
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly TrainingDbContext context;

        public ScheduleRepository(TrainingDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Schedule> GetAll()
        {
            return context.Schedules.Include(e => e.Exercises).AsNoTracking();
        }

        public Schedule? GetById(int Id)
        {
            return context.Schedules
                .Include(e => e.Exercises)
                .AsNoTracking()
                .Where(w => w.ScheduleId == Id)
                .FirstOrDefault();
        }

        public void Add(Schedule schedule)
        {
            context.Schedules.Add(schedule);
            context.SaveChanges();
        }

        public void AddMultiple(Schedule[] schedules)
        {
            foreach (Schedule schedule in schedules)
            {
                context.Schedules.Add(schedule);
            }

            context.SaveChanges();
        }

        public void Edit(Schedule selectedSchedule)
        {
            var schedule = context.Schedules
                .Include(e => e.Exercises)
                .Where(w => w.ScheduleId == selectedSchedule.ScheduleId)
                .FirstOrDefault();

            if (schedule == null) return;

            schedule.Title = selectedSchedule.Title;
            schedule.Timeslot = selectedSchedule.Timeslot;
            schedule.Weekday = selectedSchedule.Weekday;
            schedule.IsComplete = selectedSchedule.IsComplete;

            foreach (var item in schedule.Exercises)
            {
                context.Exercises.Remove(item);
            }

            schedule.Exercises = selectedSchedule.Exercises;

            context.SaveChanges();
        }

        public void Delete(Schedule selectedSchedule)
        {
            var schedule = context.Schedules
                .Include(e => e.Exercises)
                .Where(w => w.ScheduleId == selectedSchedule.ScheduleId)
                .FirstOrDefault();


            foreach (var item in schedule.Exercises)
            {
                context.Exercises.Remove(item);
            }

            context.Schedules.Remove(schedule);
            context.SaveChanges();
        }

        public DateTime GetLatestDate(short timeslot, short weekday)
        {
            return context.Schedules
                .AsNoTracking()
                .OrderBy(o => o.Date)
                .Where(w => w.Timeslot == timeslot && w.Weekday == weekday)
                .Select(s => s.Date)
                .FirstOrDefault();
        }
    }
}
