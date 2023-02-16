﻿using Microsoft.EntityFrameworkCore;

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
            return context.Schedules.Include(e=>e.Exercises);
        }

        public Schedule? GetById(int Id)
        {
            return context.Schedules
                .Include(e => e.Exercises)
                .Where(w=>w.ScheduleId == Id)
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
    }
}
