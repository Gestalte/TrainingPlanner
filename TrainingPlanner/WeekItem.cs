using System.Collections.Generic;

namespace TrainingPlanner
{
    public class WeekItem
    {
        public WeekItem(string title,int param, RelayCommand<object> editItemCommand,List<string> lineItems)
        {
            Title = title;
            EditItemCommand = editItemCommand;
            LineItems = lineItems;
            Param = default;
            IsVisible = true;
            Param = param;
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
        public int Param { get; set; }
        public bool IsVisible { get; set; }
    }
}