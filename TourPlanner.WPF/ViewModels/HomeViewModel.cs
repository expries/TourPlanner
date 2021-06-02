using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TourPlanner.BL.Services;
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

        // backing fields

        private string _searchText = string.Empty;

        private Tour _currentTour;

        private Image _image = new Image();
        
        private bool TourIsSelected => this._currentTour != null;

        // public properties
        
        public INavigator Navigator => State.Navigator.Instance;

        public ImageSource Image
        {
            get => this._image.Source;
            set
            {
                this._image.Source = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<Tour> TourList { get; set; }

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

        public HomeViewModel(ITourService tourService, IReportService reportService)
        {
            this._tourService = tourService;
            this._reportService = reportService;

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
        }

        public override void OnNavigation(object context)
        {
            this._searchText = string.Empty;
            LoadTours();
            SelectTour(context);
        }

        private void SearchTour(string parameter)
        {
            Log.Debug("Search for tour '" + this.SearchText + "' was triggered.");
            var foundTours = this._tourService.FindTours(parameter);
            this.TourList = new ObservableCollection<Tour>(foundTours);
            OnPropertyChanged(nameof(this.TourList));
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
            this.Context = this.CurrentTour;
            this.Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.NewTour);
            this.Context = null;
        }

        private async void DeleteTour(object parameter)
        {
            Log.Debug("Delete tour was triggered");
            await this._tourService.DeleteTourAsync(this.CurrentTour);
            this.CurrentTour = null;
            LoadTours();
        }
        
        private async void CreateReport(object parameter)
        {
            Log.Debug("Create report was triggered.");
            await this._reportService.CreateTourReportAsync(this.CurrentTour);
        }

        private async void DeleteTourLog(object parameter)
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

        private void EditTourLog(object parameter)
        {
            Log.Debug("Edit tour was triggered.");

            if (parameter is not TourLog tourLog)
            {
                return;
            }

            tourLog.Tour = new Lazy<Tour>(this.CurrentTour);
            this.Context = tourLog;
            this.Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.NewTourLog);
            this.Context = null;
        }

        private void CreateTourLog(object parameter)
        {
            Log.Debug("Create tour log was triggered.");
            this.Context = this.CurrentTour;
            this.Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.NewTourLog);
            this.Context = null;
        }
        
        private void CreateSummaryReport(object parameter)
        {
            this._reportService.CreateSummaryReport();
        }

        private void ExportTours(object parameter)
        {
            this._tourService.ExportTours();
        }

        private void ImportTours(object parameter)
        {
            var tours = this._tourService.ImportTours();
            this.TourList = new ObservableCollection<Tour>(tours);
            OnPropertyChanged(nameof(this.TourList));
        }

        private async void LoadTours()
        {
            var foundTours = await this._tourService.GetToursAsync();
            this.TourList = new ObservableCollection<Tour>(foundTours);
            OnPropertyChanged(nameof(this.TourList));
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