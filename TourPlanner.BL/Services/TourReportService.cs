using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using QuestPDF.Fluent;
using TourPlanner.Domain.Documents;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class TourReportService : ITourReportService
    {
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        
        public Task CreateTourReportAsync(Tour tour)
        {
            return Task.Run(() => CreateTourReport(tour));
        }

        public void CreateTourReport(Tour tour)
        {
            try
            {
                Log.Debug("Creating tour report: Opening save file dialog ...");
                
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pdf|*.pdf";
                saveFileDialog.Title = "Speicherort für den Tourreport auswählen ...";
                saveFileDialog.ShowDialog();
                
                string filePath = saveFileDialog.FileName;
                
                if (string.IsNullOrEmpty(filePath))
                {
                    Log.Debug("received empty file path for report creation.");
                    throw new BusinessException("Can't save tour report to empty path.");
                }
                
                var document = new TourReport(tour);
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