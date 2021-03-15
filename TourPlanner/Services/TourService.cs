using System.Collections.Generic;
using TourPlanner.Models;
using TourPlanner.Repositories;

namespace TourPlanner.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        
        public TourService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }
        
        public List<Tour> GetTours()
        {
            return _tourRepository.GetRoutes();
        }
        
        public void SaveRoute(Tour tour)
        {
            _tourRepository.SaveRoute(tour);
        }

        public string LoadRoutePicture(Tour tour)
        {
            return _tourRepository.LoadRoutePicture(tour);
        }
    }
}