using System.ComponentModel.DataAnnotations;

namespace TourPlanner.WPF.ValidationRules
{
    public class GreaterAttribute : ValidationAttribute
    {
        private readonly int _lowerBound;
        
        public GreaterAttribute(int lowerBound)
        {
            this._lowerBound = lowerBound;
        }
        
        public override bool IsValid(object? value)
        {
            if (value is not double doubleValue)
            {
                return true;
            }

            return doubleValue > this._lowerBound;
        }
    }
}
