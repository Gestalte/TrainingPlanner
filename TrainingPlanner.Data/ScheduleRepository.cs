using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        public IEnumerable<Schedule> GetAllForCurrentWeek(DateTime now)
        {
            var day = now.DayOfWeek;

            DateTime weekStart = now.AddDays(-((int)day));
            DateTime weekEnd = now.AddDays(6 - (int)day);

            Debug.Assert(weekStart.DayOfWeek == DayOfWeek.Sunday);
            Debug.Assert(weekEnd.DayOfWeek == DayOfWeek.Saturday);

            Func<DateTime, DateTime> scrubTime = t =>
            {
                t = t.AddHours(-t.Hour);
                t = t.AddMinutes(-t.Minute);
                t = t.AddSeconds(-t.Second);

                return t;
            };

            // Clean Times so that Start is 00.00.00 and end is 23.59.59
            weekStart = scrubTime(weekStart);
            weekEnd = scrubTime(weekEnd);

            weekEnd = weekEnd.AddHours(23);
            weekEnd = weekEnd.AddMinutes(59);
            weekEnd = weekEnd.AddSeconds(59);

            return context.Schedules
                .AsNoTracking()
                .Include(e => e.Exercises)
                .Where(w => w.Date < weekEnd && w.Date > weekStart);
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

        public void Edit(Schedule modifiedSchedule)
        {
            var schedule = context.Schedules
                .Include(e => e.Exercises)
                .Where(w => w.ScheduleId == modifiedSchedule.ScheduleId)
                .FirstOrDefault();

            if (schedule == null) return;

            bool CanSave = CheckDate(schedule, modifiedSchedule);

            if (!CanSave)
            {
                throw new AlreadyOccupiedException();
            }

            schedule.Title = modifiedSchedule.Title;
            schedule.Timeslot = modifiedSchedule.Timeslot;
            schedule.Weekday = modifiedSchedule.Weekday;
            schedule.IsComplete = modifiedSchedule.IsComplete;

            foreach (var item in schedule.Exercises)
            {
                context.Exercises.Remove(item);
            }

            schedule.Exercises = modifiedSchedule.Exercises;

            context.SaveChanges();
        }

        private bool CheckDate(Schedule schedule, Schedule modifiedSchedule)
        {
            var s = schedule;
            var m = modifiedSchedule;

            if (s.Timeslot == m.Timeslot
                && s.Weekday == m.Weekday)
            {
                return true;
            }

            bool IsOccupied = this.context.Schedules
                .Where(w => w.Timeslot == m.Timeslot && w.Weekday == m.Weekday)
                .Select(s => s.Date)
                .OrderByDescending(s => s)
                .FirstOrDefault() == DateTime.MinValue;

            return IsOccupied;
        }

        public void Delete(Schedule selectedSchedule)
        {
            var schedule = context.Schedules
                .Include(e => e.Exercises)
                .Where(w => w.ScheduleId == selectedSchedule.ScheduleId)
                .FirstOrDefault();

            if (schedule == null) return;

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
