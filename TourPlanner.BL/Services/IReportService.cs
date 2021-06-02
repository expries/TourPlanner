using System.Threading.Tasks;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface IReportService
    {
        public Task CreateTourReportAsync(Tour tour);

        public void CreateTourReport(Tour tour);

        public Task CreateSummaryReportAsync();

        public void CreateSummaryReport();
    }
}