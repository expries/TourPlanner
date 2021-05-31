using System.Threading.Tasks;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface ITourReportService
    {
        public Task CreateTourReportAsync(Tour tour);

        public void CreateTourReport(Tour tour);
    }
}