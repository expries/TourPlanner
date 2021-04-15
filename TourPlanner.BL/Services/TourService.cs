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

        private readonly ILocationRepository _locationRepository;
        
        public TourService(ITourRepository tourRepository, ILocationRepository locationRepository)
        {
            this._tourRepository = tourRepository;
            this._locationRepository = locationRepository;
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

        public void SaveTour(Tour tour)
        {
            var tourFromList = this._locationRepository.Find(tour.From);
            string tourFrom = tourFromList.FirstOrDefault();

            if (tourFrom is null)
            {
                throw new SystemException("There is no location with name ....");
            }
            
            var tourToList = this._locationRepository.Find(tour.From);
            string tourTo = tourToList.FirstOrDefault();

            if (tourTo is null)
            {
                throw new SystemException("There is no location with name ....");
            }

            tour.From = tourFrom;
            tour.To = tourTo;
            this._tourRepository.SaveTour(tour);
        }
    }
}