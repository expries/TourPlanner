using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourRepository
    {
        public List<Tour> GetTours();

        public void SaveTour(Tour tour);
    }
}