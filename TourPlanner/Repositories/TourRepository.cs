using System.Collections.Generic;
using System.Windows.Ink;
using TourPlanner.Models;

namespace TourPlanner.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly List<Tour> _tours;

        public TourRepository()
        {
            _tours = new List<Tour>
            {
                new Tour() { From = "Vienna", To = "New York", Description = "Left, Right, Right" },
                new Tour() { From = "Vienna", To = "Graz", Description = "Right, Left, Left" },
                new Tour() { From = "Vienna", To = "Vienna", Description = "Left, Left, Left, Left" }
            };
        }
        
        public List<Tour> GetTours()
        {
            return _tours;
        }

        public void SaveTour(Tour tour)
        {
            _tours.Add(tour);
        }
    }
}