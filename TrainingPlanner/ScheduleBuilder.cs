using System;
using System.Collections.Generic;
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

        public List<Schedule> LoadSchedules()
        {
            return this.scheduleRepository.GetAllForCurrentWeek(DateTime.Now)
               .Where(w => w.IsComplete == false)
               .ToList();
        }

        public void CreateSchedules
            (int numberOfOccurances
            , List<DayOfWeek> days
            , string title
            , TimeSlot timeslot
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
                        Timeslot = (short)timeslot,
                        IsComplete = isComplete,
                        Date = GetNextAvailableDate(timeslot, day),
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

        public void EditSchedule
            (string title
            , TimeSlot timeslot
            , DayOfWeek weekday
            , List<ExerciseItem> exerciseItems
            , bool isCompleted
            , int id
            )
        {
            Schedule schedule = new()
            {
                Title = title,
                Timeslot = (short)timeslot,
                Weekday = (short)weekday,
                Exercises = exerciseItems.Select(s => new Exercise
                {
                    Description = s.Description,
                    ExerciseId = s.ExerciseId
                }).ToList(),
                IsComplete = isCompleted,
                ScheduleId=id,
            };

            try
            {
                this.scheduleRepository.Edit(schedule);
            }
            catch (AlreadyOccupiedException ex)
            {
                throw ex;
            }
        }

        public void DeleteSchedule(Schedule schedule)
        {
            this.scheduleRepository.Delete(schedule);
        }

        public void FlushNewDates()
        {
            NewDates = new();
        }
        private List<DateTime> NewDates { get; set; } = new();

        public DateTime GetNextAvailableDate(TimeSlot timeSlot, DayOfWeek weekDay)
        {
            // TODO: Clean this up/Find a beter way of doing it.

            var latestDateInDB = this.scheduleRepository.GetLatestDate((short)timeSlot, (short)weekDay);

            var latestNewDate = NewDates.LastOrDefault();

            if (latestNewDate == DateTime.MinValue)
            {
                latestNewDate = DateTime.Now;
            }

            latestNewDate = latestDateInDB < latestNewDate
                ? latestNewDate
                : latestDateInDB.AddDays(7);

            if (NewDates.Contains( latestNewDate))
            {
                latestNewDate = latestNewDate.AddDays(7);
            }

            DayOfWeek day = latestNewDate.Date.DayOfWeek;

            int diff = (int)weekDay - (int)day;

            int daysToAdd = diff >= 0
                ? diff
                : 7 - Math.Abs(diff);

            var newDateToAdd = latestNewDate.AddDays(daysToAdd);

            NewDates.Add(newDateToAdd);

            return newDateToAdd;
        }
    }
}