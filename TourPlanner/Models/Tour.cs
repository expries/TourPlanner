namespace TourPlanner.Models
{
    public class Tour
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Description { get; set; }

        public Tour()
        {
            From = string.Empty;
            To = string.Empty;
            Description = string.Empty;
        }
    }
}