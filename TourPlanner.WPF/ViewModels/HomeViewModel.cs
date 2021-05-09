using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
        private readonly ITourService _tourService;
        
        private readonly IMapService _mapService;

        // backing fields
        
        private List<Tour> _tourList = new List<Tour>();

        private string _searchText = string.Empty;

        private bool TourIsSelected => this._currentTour != null;
        
        private Image _image = new Image();

        private Tour _currentTour;

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

        public ICommand SaveTourLogCommand { get; }

        public ICommand CreateReportCommand { get; }

        public ICommand SelectTourCommand { get; }
        
        public ICommand UpdateCurrentTourCommand { get; }
        
        public ICommand DeleteCurrentTourCommand { get; }

        public HomeViewModel(ITourService tourService, IMapService mapService)
        {
            this._tourService = tourService;
            this._mapService = mapService;

            this.EditTourLogCommand = new RelayCommand(EditTourLog, _ => this.TourIsSelected);
            this.SaveTourLogCommand = new RelayCommand(SaveTourLog);
            this.DeleteTourLogCommand = new RelayCommand(DeleteTourLog, _ => this.TourIsSelected);
            this.CreateReportCommand = new RelayCommand(CreateReport);
            this.SelectTourCommand = new RelayCommand(SelectTour);
            this.DeleteCurrentTourCommand = new RelayCommand(DeleteTour, _ => this.TourIsSelected);
            this.UpdateCurrentTourCommand = new RelayCommand(UpdateCurrentTour, _ => this.TourIsSelected);
        }

        public override void OnNavigation(object context)
        {
            this._searchText = string.Empty;
            LoadTours();
            SelectTour(context);
        }

        private void SearchTour(string parameter)
        {
            Debug.Print("Search for tour '" + this.SearchText + "' was triggered.");
            this.TourList = this._tourService.FindTours(parameter);
        }

        private void SelectTour(object parameter)
        {
            if (parameter is not Tour tour)
            {
                return;
            }
            
            Debug.Print("Selecting tour with name " + tour.Name);
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
            await this._tourService.DeleteTourAsync(this.CurrentTour);
            this.CurrentTour = null;
            LoadTours();
        }
        
        private void CreateReport(object parameter)
        {
            Debug.Print("Create report was triggered.");
        }
        
        private void EditTourLog(object parameter)
        {
            Debug.Print("Edit tour was triggered.");
        }

        private void DeleteTourLog(object parameter)
        {
            Debug.Print("Delete tour was triggered.");
        }

        private void SaveTourLog(object parameter)
        {
            Debug.Print("Save tour was triggered.");
        }

        private async void LoadTours()
        {
            this.TourList = await this._tourService.GetToursAsync();
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