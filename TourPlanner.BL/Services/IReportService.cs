using System.Threading.Tasks;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface IReportService
    {
        public Task CreateTourReportAsync(Tour tour, string filePath);

        public void CreateTourReport(Tour tour, string filePath);

        public Task CreateSummaryReportAsync(string filePath);

        public void CreateSummaryReport(string filePath);
    }
}