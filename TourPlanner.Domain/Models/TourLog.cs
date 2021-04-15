namespace TourPlanner.Domain.Models
{
    public class TourLog
    {
        public string TimeFrom;

        public string TimeTo;

        public int Distance;

        public TourLog()
        {
            this.TimeFrom = string.Empty;
            this.TimeTo = string.Empty;
            this.Distance = 0;
        }
    }
}
