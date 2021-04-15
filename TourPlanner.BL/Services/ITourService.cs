using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface ITourService
    {
        public List<Tour> GetTours();

        public List<Tour> FindTours(string query);

        public void SaveTour(Tour tour);
    }
}