using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
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
        
        public Task CreateTourReportAsync(Tour tour, string filePath)
        {
            return Task.Run(() => CreateTourReport(tour, filePath));
        }
        
        public Task CreateSummaryReportAsync(string filePath)
        {
            return Task.Run(() => CreateSummaryReport(filePath));
        }

        public void CreateTourReport(Tour tour, string filePath)
        {
            try
            {
                Log.Debug("Creating tour report ...");

                if (tour is null)
                {
                    Log.Error("Can't create tour report for tour that does not exist.");
                    throw new BusinessException("Fehler beim Erstellen des Tour-Reports: Es gibt die ausgewählte Tour nicht mehr.");
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    Log.Error("Received empty file path for tour report creation.");
                    throw new BusinessException("Der Tour-Report konnte nicht gespeichert werden: Angegebener Pfad ist leer.");
                }

                var document = new TourReport(tour);
                document.GeneratePdf(filePath);
                Process.Start("explorer.exe", filePath);
            }
            catch (IOException ex)
            {
                Log.Error($"Failed create tour report: {ex.Message}");
                throw new BusinessException("Fehler beim Speichern des Tour-Report: Kann nicht auf die Datei zugreifen.", ex);
            }
            
        }

        public void CreateSummaryReport(string filePath)
        {
            try
            {
                Log.Debug("Creating summary report ...");

                if (string.IsNullOrEmpty(filePath))
                {
                    Log.Error("Received empty file path for summary report creation.");
                    throw new BusinessException("Kann den Summary-Report nicht in einem leeren Pfad speichern.");
                }

                var tours = this._tourRepository.GetAll();
                var document = new SummaryReport(tours);
                document.GeneratePdf(filePath);
                Process.Start("explorer.exe", filePath);
            }
            catch (IOException ex)
            {
                Log.Error($"Failed create tour summary report: {ex.Message}");
                throw new BusinessException(
                    "Fehler beim Speichern des Tour Summary-Report: Kann nicht auf die Datei zugreifen.", ex);
            }
            catch (DataAccessException ex)
            {
                Log.Error($"Failed to create tour report: {ex.Message}");
                throw new BusinessException("Fehler beim Abrufen der Touren.", ex);
            }
        }
    }
}