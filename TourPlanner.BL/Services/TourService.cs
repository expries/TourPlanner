using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        private readonly IMapRepository _mapRepository;
        
        public TourService(ITourRepository tourRepository, IMapRepository mapRepository)
        {
            this._tourRepository = tourRepository;
            this._mapRepository = mapRepository;
        }
        
        public List<Tour> GetTours()
        {
            return this._tourRepository.GetTours();
        }

        public List<Tour> FindTours(string query)
        {
            var tours = this._tourRepository.GetTours();

            if (string.IsNullOrWhiteSpace(query))
            {
                return tours;
            }

            query = query.ToLower();
            return tours.FindAll(tour => tour.From.ToLower().Contains(query) || tour.To.ToLower().Contains(query));;
        }

        public Tour SaveTour(Tour tour)
        {
            var fromLocations = this._mapRepository.FindLocation(tour.From);
            string from = fromLocations.Select(x => x.Name).FirstOrDefault();

            if (from is null)
            {
                throw new SystemException("There is no location with name ....");
            }
            
            var toLocation = this._mapRepository.FindLocation(tour.To);
            string to = toLocation.Select(x => x.Name).FirstOrDefault();

            if (to is null)
            {
                throw new SystemException("There is no location with name ....");
            }

            tour.From = from;
            tour.To = to;
            var newTour = this._tourRepository.SaveTour(tour);
            return newTour;
        }
    }
}