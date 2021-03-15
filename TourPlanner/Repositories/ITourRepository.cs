using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Repositories
{
    public interface ITourRepository
    {
        public List<Tour> GetRoutes();

        public void SaveRoute(Tour tour);

        public string LoadRoutePicture(Tour tour);
    }
}