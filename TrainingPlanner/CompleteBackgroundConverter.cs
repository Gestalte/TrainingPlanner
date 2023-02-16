using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TrainingPlanner
{
    public class CompleteBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true)
            {
                return Brushes.DarkOrange;
            }
            else
            {
                return Brushes.Green;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
