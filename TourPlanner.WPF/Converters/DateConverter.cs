using System;
using System.Globalization;
using System.Windows.Data;

namespace TourPlanner.WPF.Converters
{
    public class DateConverter : IValueConverter
    {
        private DateTime _lastResult = DateTime.Today;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime dateTime)
            {
                return string.Empty;
            }

            if (dateTime.Equals(default(DateTime)))
            {
                dateTime = DateTime.Today;
            }

            return $"{dateTime.Day}.{dateTime.Month}.{dateTime.Year}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string stringValue)
            {
                return null;
            }

            bool tryParse = DateTime.TryParseExact(
                stringValue, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result);

            if (tryParse)
            {
                this._lastResult = result;
                return result;
            }

            return this._lastResult;
        }
    }
}