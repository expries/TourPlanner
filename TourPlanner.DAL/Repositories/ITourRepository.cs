using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourRepository
    {
        public List<Tour> GetAll();
        
        public Tour Get(int tourId);
        
        public Tour Save(Tour tour);

        public bool Delete(Tour tour);
    }
}