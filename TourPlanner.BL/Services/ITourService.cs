using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface ITourService
    {
        public Task<List<Tour>> GetToursAsync();

        public Task<List<Tour>> FindToursAsync(string query);
        
        public Task<Tour> SaveTourAsync(Tour tour);

        public Task<TourLog> SaveTourLogAsync(TourLog tourLog);
        
        public Task DeleteTourAsync(Tour tour);

        public Task DeleteTourLogAsync(TourLog tourLog);

        public Task<List<Tour>> ImportToursAsync();

        public Task ExportToursAsync();
        

        public List<Tour> GetTours();
        
        public List<Tour> FindTours(string query);
        
        public Tour SaveTour(Tour tour);
        
        public TourLog SaveTourLog(TourLog tourLog);

        public void DeleteTour(Tour tour);

        public void DeleteTourLog(TourLog tourLog);

        public List<Tour> ImportTours();

        public void ExportTours();
    }
}