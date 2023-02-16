using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace TrainingPlanner
{
    public interface IScheduleBuilder
    {
        IRelayCommand<object> EditCommand { get; set; }

        List<WeekItem> LoadSchedules();
    }
}