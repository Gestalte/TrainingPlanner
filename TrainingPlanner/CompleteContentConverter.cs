﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace TrainingPlanner
{
    public class CompleteContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true)
            {
                return "Mark Incomplete";
            }
            else
            {
                return "Mark Complete";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
