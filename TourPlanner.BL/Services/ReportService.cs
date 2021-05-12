using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using TourPlanner.Domain.Documents;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class ReportService : IReportService
    {
        public Task CreateTourReportAsync(Tour tour, string filePath)
        {
            return Task.Run(() => CreateTourReport(tour, filePath));
        }

        public void CreateTourReport(Tour tour, string filePath)
        {
            var document = new TourReport(tour);
            document.GeneratePdf(filePath);
            Process.Start("explorer.exe", filePath);
        }
    }
}