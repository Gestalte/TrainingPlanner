using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static TrainingPlanner.MainWindowViewModel;

namespace TrainingPlanner
{
    public class AddEditViewVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WindowView w)
            {
                if (w == WindowView.AddEditview)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            else
            {
                throw new Exception("No Weekview selected.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AMPMEnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? parameter : Binding.DoNothing;
        }
    }
}
