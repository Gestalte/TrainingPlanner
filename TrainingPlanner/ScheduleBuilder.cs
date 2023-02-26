using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    public class ScheduleBuilder : IScheduleBuilder
    {
        private readonly IScheduleRepository scheduleRepository;

        public ScheduleBuilder(IScheduleRepository scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;
        }

        // TODO: Load only schedules in the current week.
        public List<Schedule> LoadSchedules()
        {
            var weekItemArr = new WeekItem[14];

            for (int i = 0; i < weekItemArr.Length; i++)
            {
                weekItemArr[i] = new WeekItem();
            }

            return this.scheduleRepository.GetAll()
               .Where(w => w.IsComplete == false)
               .DistinctBy(d => (d.Weekday, d.Timeslot))
               .ToList();
        }

        public void CreateSchedules
            ( int numberOfOccurances
            , List<DayOfWeek> days
            , string title
            , short timeslot
            , List<ExerciseItem> exercises
            , bool isComplete = false
            )
        {
            List<Schedule> schedules = new();

            for (int i = 0; i < numberOfOccurances; i++)
            {
                foreach (var day in days)
                {
                    Schedule schedule = new()
                    {
                        Title = title,
                        Weekday = (short)day,
                        Timeslot = timeslot,
                        IsComplete = isComplete,
                        Date = GetNextAvailableDate(day),
                        Exercises = exercises.Select(s => new Exercise
                        {
                            Description = s.Description
                        }).ToList()
                    };

                    schedules.Add(schedule);
                }
            }

            this.scheduleRepository.AddMultiple(schedules.ToArray());
        }

        private void EditSchedule
            (string title
            , short timeslot
            , short weekday
            , List<ExerciseItem> exerciseItems
            , bool isCompleted
            )
        {
            Schedule schedule = new()
            {
                Title = title,
                Timeslot = timeslot,
                Weekday = weekday,
                Exercises = exerciseItems.Select(s => new Exercise
                {
                    Description = s.Description,
                    ExerciseId = s.ExerciseId
                }).ToList(),
                IsComplete = isCompleted,
                Date = DateTime.Now, // TODO: Figure out what should happen here.                
            };

            this.scheduleRepository.Edit(schedule);
        }

        void DeleteSchedule(Schedule schedule)
        {
            this.scheduleRepository.Delete(schedule);
        }

        public DateTime GetNextAvailableDate(DayOfWeek weekDay)
        {
            var currentDate = GetLastDate();

            // TODO: Use DayOfWeek instead of custom version.
            DayOfWeek day = currentDate.Date.DayOfWeek;

            int diff = (int)weekDay - (int)day;

            int daysToAdd = diff >= 0
                ? diff
                : 7 - Math.Abs(diff);

            return currentDate.AddDays(daysToAdd);
        }

        public DateTime GetLastDate()
        {
            var latestDate = this.scheduleRepository.GetLatestDate();

            if (latestDate < DateTime.Now)
            {
                latestDate = DateTime.Now;
            }

            return latestDate;
        }
    }
}