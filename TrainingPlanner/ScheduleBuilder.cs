using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    public class ScheduleBuilder : IScheduleBuilder
    {
        private readonly IScheduleRepository scheduleRepository;
        public IRelayCommand<object> EditCommand { get; set; } = null!; // Use property injection to set this.

        public ScheduleBuilder(IScheduleRepository scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;
        }

        // TODO: Load only schedules in the current week.
        public List<WeekItem> LoadSchedules()
        {
            Func<Schedule, WeekItem> makeWeekItem = s
                => new WeekItem(s.Title, s.ScheduleId, EditCommand, s.Exercises.Select(s => s.Description).ToList());

            if (EditCommand == null)
            {
                throw new ArgumentNullException(nameof(EditCommand));
            }

            var weekItemArr = new WeekItem[14];

            for (int i = 0; i < weekItemArr.Length; i++)
            {
                weekItemArr[i] = new WeekItem();
            }

            this.scheduleRepository.GetAll()
                .Where(w => w.IsComplete == false)
                .DistinctBy(d => (d.Weekday, d.Timeslot))
                .ToList()
                .ForEach(f => AddItem(ref weekItemArr, makeWeekItem(f), (WeekDay)f.Weekday, (TimeSlot)f.Timeslot));

            return weekItemArr.ToList();
        }

        private WeekItem[] AddItem(ref WeekItem[] weekItems, WeekItem newItem, WeekDay weekDay, TimeSlot timeSlot)
        {
            int index = (int)timeSlot == 1 ? (int)weekDay : (int)weekDay + 7;

            weekItems[index] = newItem;

            return weekItems;
        }
    }
}