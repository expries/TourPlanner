using System;
using System.Globalization;
using System.Windows.Data;

namespace TourPlanner.WPF.Converters
{
    public class DoubleConverter : IValueConverter
    {
        private double _lastResult = 0; 
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double doubleValue)
            {
                return string.Empty;
            }

            return doubleValue.ToString(CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            bool tryParse = double.TryParse(stringValue, out double result);

            if (tryParse)
            {
                this._lastResult = result;
                return result;
            }
            
            return this._lastResult;
        }
    }
}