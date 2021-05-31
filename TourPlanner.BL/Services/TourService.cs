using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        private readonly ITourLogRepository _tourLogRepository;

        private readonly IMapRepository _mapRepository;

        private readonly IRouteImageRepository _imageRepository;

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
            return this._tourRepository.GetAll();
        }
        
        public List<Tour> FindTours(string query)
        {
            var tours = this._tourRepository.GetAll();

            if (string.IsNullOrWhiteSpace(query))
            {
                return tours;
            }

            query = query.ToLower();

            tours = tours.FindAll(tour => {
                bool fromContainsQuery = tour.From.ToLower().Contains(query);
                bool toContainsQuery = tour.To.ToLower().Contains(query);
                bool nameContainsQuery = tour.Name.ToLower().Contains(query);
                bool descriptionContainsQuery = tour.Description.ToLower().Contains(query);

                bool logContainsQuery = tour.TourLogs.Value.Any(tourLog => {
                    bool timeFromContainsQuery = tourLog.Date.ToString().Contains(query);
                    bool difficultyContainsString = tourLog.Difficulty.ToString().Contains(query);
                    bool distanceContainsString = tourLog.Distance.ToString().Contains(query);
                    bool durationContainsString = tourLog.Duration.ToString().Contains(query);
                    bool ratingContainsString = tourLog.Rating.ToString().Contains(query);
                    bool temperatureContainsString = tourLog.Temperature.ToString().Contains(query);
                    bool weatherContainsString = tourLog.Weather.ToString().Contains(query);
                    bool averageSpeedContainsString = tourLog.AverageSpeed.ToString().Contains(query);
                    bool dangerLevelContainsString = tourLog.DangerLevel.ToString().Contains(query);
                    
                    return timeFromContainsQuery || difficultyContainsString || distanceContainsString || 
                           durationContainsString || ratingContainsString || temperatureContainsString || 
                           weatherContainsString || averageSpeedContainsString || dangerLevelContainsString;
                });

                return fromContainsQuery || toContainsQuery || nameContainsQuery || descriptionContainsQuery || logContainsQuery;
            });
            
            return tours;
        }

        public Tour SaveTour(Tour tour)
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
                return new Tour();
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
        
        public TourLog SaveTourLog(TourLog tourLog)
        {
            return this._tourLogRepository.Save(tourLog);
        }
        
        public void DeleteTour(Tour tour)
        {
            if (this._imageRepository.Get(tour.ImagePath) is not null)
            {
                this._imageRepository.Delete(tour.ImagePath);
            }
            
            this._tourLogRepository.DeleteByTour(tour);
            this._tourRepository.Delete(tour);
        }

        public void DeleteTourLog(TourLog tourLog)
        {
            this._tourLogRepository.Delete(tourLog);
        }
    }
}