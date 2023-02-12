using System.Collections.Generic;

namespace TrainingPlanner
{
    public class WeekItem
    {
        public WeekItem(string title, RelayCommand<object> editItemCommand,List<string> lineItems, (WeekDay weekDay, TimeSlot timeSlot) param,bool isVisible)
        {
            Title = title;
            EditItemCommand = editItemCommand;
            LineItems = lineItems;
            Param = param;
            IsVisible = isVisible;
        }

        public WeekItem()
        {
            Title = "";
            EditItemCommand = new RelayCommand<object>(a => { });
            LineItems = new List<string>();
            Param = default;
            IsVisible = false;
        }

        public string Title { get; set; }
        public RelayCommand<object> EditItemCommand { get; set; }
        public List<string> LineItems { get; set; }
        public (WeekDay weekDay,TimeSlot timeSlot) Param { get; set; }
        public bool IsVisible { get; set; }
    }
}