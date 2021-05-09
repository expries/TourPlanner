using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        private readonly IMapRepository _mapRepository;

        private readonly string _imageDirectory;

        public TourService(ITourRepository tourRepository, IMapRepository mapRepository, IConfiguration configuration)
        {
            this._tourRepository = tourRepository;
            this._mapRepository = mapRepository;
            this._imageDirectory = configuration.GetSection("Paths")["ImageDirectory"];
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

        public bool DeleteTour(Tour tour)
        {
            if (File.Exists(tour.ImagePath))
            {
                File.Delete(tour.ImagePath);
            }
            
            return this._tourRepository.DeleteTour(tour);
        }

        public Task<bool> DeleteTourAsync(Tour tour)
        {
            return Task.Run(() => DeleteTour(tour));
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
            Console.WriteLine(query);

            tours = tours.FindAll(tour => {
                bool fromContainsQuery = tour.From.ToLower().Contains(query);
                bool toContainsQuery = tour.To.ToLower().Contains(query);
                bool nameContainsQuery = tour.Name.ToLower().Contains(query);
                bool descriptionContainsQuery = tour.Description.ToLower().Contains(query);

                bool logContainsQuery = tour.TourLogs.Value.Any(tourLog => {
                    bool timeFromContainsQuery = tourLog.TimeFrom.ToString().Contains(query);
                    bool timeToContainsQuery = tourLog.TimeTo.ToString().Contains(query);
                    return timeFromContainsQuery || timeToContainsQuery;
                });

                return fromContainsQuery || toContainsQuery || nameContainsQuery || descriptionContainsQuery || logContainsQuery;
            });

            Console.WriteLine("returning tours");
            return tours;
        }

        public Tour SaveTour(Tour tour)
        {
            // clean up old tour if updating
            var savedTour = this._tourRepository.GetTour(tour.TourId);

            if (savedTour is not null && File.Exists(savedTour.ImagePath))
            {
                File.Delete(savedTour.ImagePath);
            }
            
            // query api
            var route = this._mapRepository.GetDirection(tour.From, tour.To);
            tour.Distance = route.Route.Distance;

            if (route.Info.Statuscode != 0)
            {
                return new Tour();
            }

            byte[] imageData = this._mapRepository.GetImage(route.Route.SessionId, route.Route.BoundingBox, 400, 300);

            // save image
            string imagePath;

            do
            {
                imagePath = this._imageDirectory + "\\" + Guid.NewGuid() + ".png";
            } 
            while (File.Exists(imagePath));
            
            File.WriteAllBytes(imagePath, imageData);
            tour.ImagePath = imagePath;

            if (string.IsNullOrEmpty(tour.Description))
            {
                tour.Description = "Keine Beschreibung";
            }
            
            // save tour to db
            var newTour = this._tourRepository.SaveTour(tour);
            return newTour;
        }
    }
}