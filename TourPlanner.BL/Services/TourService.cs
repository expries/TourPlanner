using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        private readonly ITourLogRepository _tourLogRepository;

        private readonly IMapRepository _mapRepository;

        private readonly IRouteImageRepository _imageRepository;
        
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public TourService(ITourRepository tourRepository, ITourLogRepository tourLogRepository, 
                           IMapRepository mapRepository, IRouteImageRepository routeImageRepository)
        {
            this._tourRepository = tourRepository;
            this._tourLogRepository = tourLogRepository;
            this._mapRepository = mapRepository;
            this._imageRepository = routeImageRepository;
        }

        public Task<List<Tour>> GetToursAsync()
        {
            return Task.Run(GetTours);
        }
        
        public Task<List<Tour>> FindToursAsync(string query)
        {
            return Task.Run(() => FindToursAsync(query));
        }
        
        public Task<Tour> SaveTourAsync(Tour tour)
        {
            return Task.Run(() => SaveTour(tour));
        }
        
        public Task<TourLog> SaveTourLogAsync(TourLog tourLog)
        {
            return Task.Run(() => SaveTourLog(tourLog));
        }

        public Task DeleteTourAsync(Tour tour)
        {
            return Task.Run(() => DeleteTour(tour));
        }

        public Task DeleteTourLogAsync(TourLog tourLog)
        {
            return Task.Run(() => DeleteTourLog(tourLog));
        }

        public List<Tour> GetTours()
        {
            try
            {
                return this._tourRepository.GetAll();
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to get tours: {ex.Message}");
                throw new BusinessException("Failed to get tours", ex);
            }
        }
        
        public List<Tour> FindTours(string query)
        {
            try
            {
                Log.Debug($"Searching for tours with query '{query}'");
                
                var tours = this._tourRepository.GetAll();

                if (string.IsNullOrWhiteSpace(query))
                {
                    return tours;
                }

                query = query.ToLower();

                tours = tours.FindAll(tour =>
                {
                    var tourMatches = new List<bool>
                    {
                        tour.From.ToLower().Contains(query),
                        tour.To.ToLower().Contains(query),
                        tour.Name.ToLower().Contains(query),
                        tour.Description.ToLower().Contains(query),
                    };
                        
                    var logMatchLists = tour.TourLogs.Value.Select(tourLog => new List<bool>
                    {
                        tourLog.Date.ToString().Contains(query),
                        tourLog.Difficulty.ToString().Contains(query),
                        tourLog.Distance.ToString().Contains(query),
                        tourLog.Duration.ToString().Contains(query),
                        tourLog.Rating.ToString().Contains(query),
                        tourLog.Temperature.ToString().Contains(query),
                        tourLog.Weather.ToString().Contains(query),
                        tourLog.AverageSpeed.ToString().Contains(query),
                        tourLog.DangerLevel.ToString().Contains(query),
                    });
                    
                    return tourMatches.Any() || logMatchLists.Any(logMatches => logMatches.Any());
                });
                
                Log.Debug($"Search returned {tours.Count} tours");
                return tours;   
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to search for tour: {ex.Message}");
                throw new BusinessException("Failed to search for tours", ex);
            }
        }

        public Tour SaveTour(Tour tour)
        {
            try
            {
                // clean up old image if updating
                var savedTour = this._tourRepository.Get(tour.TourId);

                if (savedTour is not null)
                {
                    this._imageRepository.Delete(savedTour.ImagePath);
                }
            
                // query api
                var routeResponse = this._mapRepository.GetRoute(tour.From, tour.To);
                var route = routeResponse.Route;
                tour.Distance = route.Distance;

                if (routeResponse.Info.Statuscode != 0)
                {
                    throw new BusinessException("Could not find route for the given locations");
                }

                byte[] imageData = this._mapRepository.GetImage(route.SessionId, route.BoundingBox, 400, 300);

                // save image
                tour.ImagePath = this._imageRepository.Save(imageData);

                if (string.IsNullOrEmpty(tour.Description))
                {
                    tour.Description = "Keine Beschreibung";
                }
            
                // save tour to db
                return this._tourRepository.Save(tour);   
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to save tour: {ex.Message}");
                throw new BusinessException("Failed to save tour", ex);
            }
        }
        
        public TourLog SaveTourLog(TourLog tourLog)
        {
            try
            {
                if (tourLog.Tour.Value is null)
                {
                    throw new BusinessException("Tour log is not associated with tour");
                }
                
                return this._tourLogRepository.Save(tourLog);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to search for tour log: {ex.Message}");
                throw new BusinessException("Failed to save tour log", ex);
            }
        }
        
        public void DeleteTour(Tour tour)
        {
            try
            {
                if (this._imageRepository.Get(tour.ImagePath) is not null)
                {
                    this._imageRepository.Delete(tour.ImagePath);
                }

                this._tourLogRepository.DeleteByTour(tour);
                this._tourRepository.Delete(tour);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed delete tour: {ex.Message}");
                throw new BusinessException("Failed to delete tour", ex);
            }
        }

        public void DeleteTourLog(TourLog tourLog)
        {
            try
            {
                this._tourLogRepository.Delete(tourLog);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to search for tour log: {ex.Message}");
                throw new BusinessException("Failed to delete tour log", ex);
            }
        }
    }
}