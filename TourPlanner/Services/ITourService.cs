using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Services
{
    public interface ITourService
    {
        public List<Tour> GetTours();

        public void SaveRoute(Tour tour);

        public string LoadRoutePicture(Tour tour);
    }
}