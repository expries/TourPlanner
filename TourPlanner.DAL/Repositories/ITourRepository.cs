using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourRepository
    {
        public Tour GetTour(int tourId);
        
        public List<Tour> GetTours();

        public Tour CreateTour(Tour tour);

        public bool DeleteTour(Tour tour);

        public Tour SaveTour(Tour tour);
    }
}