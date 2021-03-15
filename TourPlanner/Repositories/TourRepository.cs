using System.Collections.Generic;
using System.Windows.Media.Imaging;
using TourPlanner.Models;

namespace TourPlanner.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly List<Tour> _routes;

        public TourRepository()
        {
            _routes = new List<Tour>();
        }
        
        public List<Tour> GetRoutes()
        {
            return _routes;
        }

        public void SaveRoute(Tour tour)
        {
            _routes.Add(tour);
        }

        public string LoadRoutePicture(Tour tour)
        {
            return string.Empty;
        }
    }
}