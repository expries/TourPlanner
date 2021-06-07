using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TourPlanner.BL.Services;
using TourPlanner.Domain.Exceptions;
using TourPlanner.WPF.State;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        
        private readonly ITourService _tourService;

        private readonly IReportService _reportService;

        private readonly ITourListObservable _tourListObservable;

        // backing fields

        private string _searchText = string.Empty;

        private Tour _currentTour;

        private readonly Image _image = new Image();
        
        private bool TourIsSelected => this._currentTour is not null;

        private List<Tour> _tourList = new List<Tour>();

        // public properties
        
        public static INavigator Navigator => State.Navigator.Instance;

        public ImageSource Image
        {
            get => this._image.Source;
            set
            {
                this._image.Source = value;
                OnPropertyChanged();
            }
        }

        public List<Tour> TourList
        {
            get => this._tourList;
            set
            {
                this._tourList = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => this._searchText;
            set
            {
                this._searchText = value;
                OnPropertyChanged();
                SearchTour(value);
            }
        }

        public Tour CurrentTour
        {
            get => this._currentTour;
            set
            {
                this._currentTour = value;
                OnPropertyChanged();
            }
        }

        // public commands
        
        public ICommand DeleteTourLogCommand { get; }

        public ICommand EditTourLogCommand { get; }

        public ICommand CreateTourLogCommand { get; }

        public ICommand CreateReportCommand { get; }

        public ICommand SelectTourCommand { get; }
        
        public ICommand UpdateCurrentTourCommand { get; }
        
        public ICommand DeleteCurrentTourCommand { get; }
        
        public ICommand CreateSummaryReportCommand { get; }
        
        public ICommand ExportToursCommand { get; }
        
        public ICommand ImportToursCommand { get; }

        public HomeViewModel(ITourService tourService, IReportService reportService, 
                             ITourListObservable tourListObservable)
        {
            this._tourService = tourService;
            this._reportService = reportService;
            this._tourListObservable = tourListObservable;
            this._tourListObservable.Subscribe(tourList => this.TourList = tourList);

            this.SelectTourCommand = new RelayCommand(SelectTour);
            this.DeleteCurrentTourCommand = new RelayCommand(DeleteTour, _ => this.TourIsSelected);
            this.UpdateCurrentTourCommand = new RelayCommand(UpdateCurrentTour, _ => this.TourIsSelected);
            this.CreateReportCommand = new RelayCommand(CreateReport, _ => this.TourIsSelected);

            this.EditTourLogCommand = new RelayCommand(EditTourLog, _ => this.TourIsSelected);
            this.CreateTourLogCommand = new RelayCommand(CreateTourLog, _ => this.TourIsSelected);
            this.DeleteTourLogCommand = new RelayCommand(DeleteTourLog, _ => this.TourIsSelected);

            this.CreateSummaryReportCommand = new RelayCommand(CreateSummaryReport);
            this.ExportToursCommand = new RelayCommand(ExportTours);
            this.ImportToursCommand = new RelayCommand(ImportTours);

            LoadTours();
        }

        public void Accept(Tour tour)
        {
            this._searchText = string.Empty;
            SelectTour(tour);
        }

        private void SearchTour(string parameter)
        {
            try
            {
                Log.Debug("Search for tour '" + this.SearchText + "' was triggered.");
                this.TourList = this._tourService.FindTours(parameter);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void SelectTour(object parameter)
        {
            if (parameter is not Tour tour)
            {
                return;
            }
            
            Log.Debug("Selecting tour with name " + tour.Name);
            this.CurrentTour = tour;
            SetImage(this.CurrentTour.Image);
        }

        private void UpdateCurrentTour(object parameter)
        {
            Navigator.UpdateCurrentViewModel(ViewType.NewTour, this.CurrentTour);
        }

        private async void DeleteTour(object parameter)
        {
            try
            {
                Log.Debug("Delete tour was triggered");
                await this._tourService.DeleteTourAsync(this.CurrentTour);
                this.CurrentTour = null;
                LoadTours();
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }
        
        private async void CreateReport(object parameter)
        {
            try
            {
                Log.Debug("Create report was triggered.");
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pdf|*.pdf";
                saveFileDialog.Title = "Speicherort für den Tourreport auswählen ...";
                saveFileDialog.ShowDialog();

                string filePath = saveFileDialog.FileName;
                this._reportService.CreateTourReport(this.CurrentTour, filePath);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private async void DeleteTourLog(object parameter)
        {
            try
            {
                Log.Debug("Delete tour log was triggered.");

                if (parameter is not TourLog tourLog)
                {
                    return;
                }
            
                await this._tourService.DeleteTourLogAsync(tourLog);
                this.CurrentTour.TourLogs.Value.RemoveAll(x => x.TourLogId == tourLog.TourLogId);
                var tour = this.CurrentTour;
                this.CurrentTour = null;
                this.CurrentTour = tour;
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void EditTourLog(object parameter)
        {
            try
            {
                Log.Debug("Edit tour was triggered.");

                if (parameter is not TourLog tourLog)
                {
                    return;
                }

                tourLog.Tour = new Lazy<Tour>(this.CurrentTour);
                Navigator.UpdateCurrentViewModel(ViewType.NewTourLog, tourLog);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void CreateTourLog(object parameter)
        {
            Log.Debug("Create tour log was triggered.");
            Navigator.UpdateCurrentViewModel(ViewType.NewTourLog, this.CurrentTour);
        }
        
        private void CreateSummaryReport(object parameter)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pdf|*.pdf";
                saveFileDialog.Title = "Speicherort für den Tourreport auswählen ...";
                saveFileDialog.ShowDialog();

                string filePath = saveFileDialog.FileName;
                this._reportService.CreateSummaryReport(filePath);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void ExportTours(object parameter)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Wähle aus, wohin der Tour-Export gespeichert werden soll ...";
                saveFileDialog.Filter = "JSON|*.json";
                saveFileDialog.ShowDialog();
                
                string filePath = saveFileDialog.FileName;
                this._tourService.ExportTours(filePath);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void ImportTours(object parameter)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Wähle eine Datei für den Tour-Import aus ...";
                openFileDialog.Filter = "JSON|*.json";
                openFileDialog.ShowDialog();

                string filePath = openFileDialog.FileName;
                this.TourList = this._tourService.ImportTours(filePath);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private async void LoadTours()
        {
            try
            {
                var tours = await this._tourService.GetToursAsync();
                this._tourListObservable.Update(tours);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void SetImage(byte[] imageData)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.None;
            image.CacheOption = BitmapCacheOption.Default;
            image.StreamSource = new MemoryStream(imageData);
            image.EndInit();
            this.Image = image;
        }
    }
}