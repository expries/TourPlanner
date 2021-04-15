using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly List<Tour> _tours;

        public TourRepository()
        {
            this._tours = new List<Tour>
            {
                new Tour(GetTourLogs) { From = "Vienna", To = "New York", Description = "Left, Right, Right" },
                new Tour(GetTourLogs) { From = "Vienna", To = "Graz", Description = "Right, Left, Left" },
                new Tour(GetTourLogs) { From = "Vienna", To = "Vienna", Description = "Left, Left, Left, Left" }
            };
        }

        public List<Tour> GetTours()
        {
            return this._tours;
        }

        public void SaveTour(Tour tour)
        {
            this._tours.Add(tour);
        }

        private List<TourLog> GetTourLogs(Tour tour)
        {
            var logList = new List<TourLog>
            {
                new TourLog { TimeFrom = tour.From, TimeTo = "000", Distance = 1000 },
                new TourLog { TimeFrom = tour.From, TimeTo = "000", Distance = 2000 },
                new TourLog { TimeFrom = tour.From, TimeTo = "000", Distance = 3000 }
            };

            return logList;
        }
    }
}