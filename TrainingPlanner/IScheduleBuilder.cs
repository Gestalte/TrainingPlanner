using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    public interface IScheduleBuilder
    {
        List<Schedule> LoadSchedules();

        void CreateSchedules
            (int numberOfOccurances
            , List<DayOfWeek> days
            , string title
            , TimeSlot timeslot
            , List<ExerciseItem> exercises
            , bool isComplete = false
            );

        void EditSchedule
            (string title
            , TimeSlot timeslot
            , DayOfWeek weekday
            , List<ExerciseItem> exerciseItems
            , bool isCompleted
            , int id
            );

        void DeleteSchedule(Schedule schedule);

        DateTime GetNextAvailableDate(TimeSlot timeSlot, DayOfWeek weekDay);

        void FlushNewDates();
    }
}