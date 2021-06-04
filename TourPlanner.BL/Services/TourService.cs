using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        private readonly ITourLogRepository _tourLogRepository;

        private readonly IRouteRepository _routeRepository;

        private readonly IRouteImageRepository _imageRepository;
        
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public TourService(ITourRepository tourRepository, ITourLogRepository tourLogRepository, 
                           IRouteRepository routeRepository, IRouteImageRepository routeImageRepository)
        {
            this._tourRepository = tourRepository;
            this._tourLogRepository = tourLogRepository;
            this._routeRepository = routeRepository;
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

        public Task<List<Tour>> ImportToursAsync()
        {
            return Task.Run(ImportTours);
        }

        public Task ExportToursAsync()
        {
            return Task.Run(ExportTours);
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
                throw new BusinessException("Fehler beim Abrufen der Touren.", ex);
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
                    
                    return tourMatches.Any(x => x) || 
                           logMatchLists.Any(logMatches => logMatches.Any(x => x));
                });
                
                Log.Debug($"Search returned {tours.Count} tours");
                return tours;   
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to search for tour: {ex.Message}");
                throw new BusinessException("Fehler bei der Tour-Suche.", ex);
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
                var routeResponse = this._routeRepository.GetRoute(tour.From, tour.To);
                var route = routeResponse.Route;
                tour.Distance = route.Distance;

                if (routeResponse.Info.Statuscode != 0)
                {
                    Log.Error($"Could not find route for given locations (From: {tour.From}, To: {tour.To}):" 
                             + string.Join("; ", routeResponse.Info.Messages));
                    
                    throw new BusinessException("Es konnte keine Route für diese Orte gefunden werden.");
                }

                byte[] imageData = this._routeRepository.GetImage(route.SessionId, route.BoundingBox, 400, 300);

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
                throw new BusinessException("Fehler beim Speichern der Tour.", ex);
            }
        }
        
        public TourLog SaveTourLog(TourLog tourLog)
        {
            try
            {
                if (tourLog.Tour.Value is null)
                {
                    throw new BusinessException("Tourlog ist mit keiner Tour verbunden.");
                }
                
                return this._tourLogRepository.Save(tourLog);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to save tour log: {ex.Message}");
                throw new BusinessException("Fehler beim Speichern des Tourlogs.", ex);
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
                throw new BusinessException("Fehler beim Löschen der Tour.", ex);
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
                throw new BusinessException("Fehler beim Löschen des Tourlogs.", ex);
            }
        }

        public List<Tour> ImportTours()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Wähle eine Datei für den Tour-Import aus ...";
                openFileDialog.Filter = "JSON|*.json";
                openFileDialog.ShowDialog();

                string filePath = openFileDialog.FileName;

                if (!File.Exists(filePath))
                {
                    throw new BusinessException("Datei für den Tour-Import konnte nicht gefunden werden.");
                }

                string fileContent = File.ReadAllText(filePath);
                var toursToImport = JsonConvert.DeserializeObject<List<Tour>>(fileContent);

                if (toursToImport is null)
                {
                    throw new BusinessException("Die Importdatei hat ein ungültiges Format.");
                }

                toursToImport.ForEach(ImportTour);
                return this._tourRepository.GetAll();   
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to import tours: {ex.Message}");
                throw new BusinessException("Fehler beim Tour-Import.", ex);
            }
        }

        public void ExportTours()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Wähle aus, wohin der Tour-Export gespeichert werden soll ...";
                saveFileDialog.Filter = "JSON|*.json";
                saveFileDialog.ShowDialog();

                var tours = this._tourRepository.GetAll();
                string toursJson = JsonConvert.SerializeObject(tours);
                string fileName = saveFileDialog.FileName;

                File.WriteAllText(fileName, toursJson);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to export tours: {ex.Message}");
                throw new BusinessException("Fehler beim Tour-Export.", ex);
            }
        }

        private void ImportTour(Tour tour)
        {
            byte[] image = this._imageRepository.Get(tour.ImagePath);

            if (image != tour.Image)
            {
                this._imageRepository.Delete(tour.ImagePath);
                tour.ImagePath = this._imageRepository.Save(tour.Image);
            }
            
            tour = this._tourRepository.Save(tour);

            tour.TourLogs.Value.ForEach(tourLog =>
            {
                tourLog.Tour = new Lazy<Tour>(() => tour);
                this._tourLogRepository.Save(tourLog);
            });
        }
    }
}