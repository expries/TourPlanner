using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface ITourService
    {
        public List<Tour> GetTours();

        public Task<List<Tour>> GetToursAsync();

        public List<Tour> FindTours(string query);

        public Task<List<Tour>> FindToursAsync(string query);

        public Tour SaveTour(Tour tour);

        public Task<Tour> SaveTourAsync(Tour tour);

        public bool DeleteTour(Tour tour);

        public Task<bool> DeleteTourAsync(Tour tour);

        public Task<Tour> UpdateTourAsync(Tour tour);

        public Tour UpdateTour(Tour tour);
    }
}