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
            return _tourRepository.GetTours();
        }

        public List<Tour> FindTours(string query)
        {
            var tours = _tourRepository.GetTours();

            if (string.IsNullOrWhiteSpace(query))
            {
                return tours;
            }

            query = query.ToLower();
            return tours.FindAll(tour => tour.From.ToLower().Contains(query) || tour.To.ToLower().Contains(query));
        }

        public void SaveTour(Tour tour)
        {
            _tourRepository.SaveTour(tour);
        }
    }
}