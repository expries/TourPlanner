using System.Globalization;
using System.Windows.Controls;

namespace TourPlanner.WPF.ValidationRules
{
    class RequiredRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;

            if (str is null)
            {
                return new ValidationResult(false, "Can't validate this field.");
            }

            if (str.Trim().Length < 1)
            {
                return new ValidationResult(false, "This field is required.");
            }
            
            return ValidationResult.ValidResult;
        }
    }
}
