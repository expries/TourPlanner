using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using QuestPDF.Fluent;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Documents;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class ReportService : IReportService
    {
        private readonly ITourRepository _tourRepository;
        
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public ReportService(ITourRepository tourRepository)
        {
            this._tourRepository = tourRepository;
        }
        
        public Task CreateTourReportAsync(Tour tour)
        {
            return Task.Run(() => CreateTourReport(tour));
        }
        
        public Task CreateSummaryReportAsync()
        {
            return Task.Run(CreateSummaryReport);
        }

        public void CreateTourReport(Tour tour)
        {
            Log.Debug("Creating tour report: Opening save file dialog ...");
                
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf|*.pdf";
            saveFileDialog.Title = "Speicherort für den Tourreport auswählen ...";
            saveFileDialog.ShowDialog();
                
            string filePath = saveFileDialog.FileName;
                
            if (string.IsNullOrEmpty(filePath))
            {
                Log.Error("Received empty file path for tour report creation.");
                throw new BusinessException("Can't save tour report to empty path.");
            }
                
            var document = new TourReport(tour);
            document.GeneratePdf(filePath);
            Process.Start("explorer.exe", filePath);
        }

        public void CreateSummaryReport()
        {
            try
            {
                Log.Debug("Creating summary report: Opening save file dialog ...");
                
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pdf|*.pdf";
                saveFileDialog.Title = "Speicherort für den Tourreport auswählen ...";
                saveFileDialog.ShowDialog();
                
                string filePath = saveFileDialog.FileName;
                
                if (string.IsNullOrEmpty(filePath))
                {
                    Log.Error("Received empty file path for summary report creation.");
                    throw new BusinessException("Can't save summary report to empty path.");
                }

                var tours = this._tourRepository.GetAll();
                var document = new SummaryReport(tours);
                document.GeneratePdf(filePath);
                Process.Start("explorer.exe", filePath);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to create tour report: {ex.Message}");
                throw new BusinessException("Failed to create tour report", ex);
            }
        }
    }
}