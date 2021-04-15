using System;
using System.Collections.Generic;

namespace TourPlanner.Domain.Models
{
    public class Tour
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Description { get; set; }
        
        public List<TourLog> TourLogs => this.LazyTourLogs.Value;

        private Lazy<List<TourLog>> LazyTourLogs { get; set; }
        
        private Func<Tour, List<TourLog>> TourLogGetter { get; set; }

        public Tour(Func<Tour, List<TourLog>> tourLogSource = null)
        {
            this.From = string.Empty;
            this.To = string.Empty;
            this.Description = string.Empty;
            this.TourLogGetter = tourLogSource ?? DefaultGetTours;
            this.LazyTourLogs = new Lazy<List<TourLog>>(GetTourLogs);
        }

        private List<TourLog> GetTourLogs()
        {
            return this.TourLogGetter.Invoke(this);
        }

        private static List<TourLog> DefaultGetTours(Tour tour)
        {
            return new List<TourLog>
            {
                new TourLog { TimeFrom = "00", TimeTo = "11", Distance = 100 },
                new TourLog { TimeFrom = "22", TimeTo = "33", Distance = 200 },
                new TourLog { TimeFrom = "44", TimeTo = "55", Distance = 300 }
            };
        }
    }
}