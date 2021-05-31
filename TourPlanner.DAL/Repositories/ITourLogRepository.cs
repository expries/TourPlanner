using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourLogRepository
    {
        public TourLog Get(int tourLogId);

        public TourLog Save(TourLog tourLog);

        public bool Delete(TourLog tourLog);
        
        public bool DeleteByTour(Tour tour);
    }
}