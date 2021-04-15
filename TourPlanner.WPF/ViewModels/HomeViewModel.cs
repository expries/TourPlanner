using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.WPF.State;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly ITourService _tourService;
    
        private List<Tour> _tourList = new List<Tour>();

        private readonly TourLog _log = new TourLog();

        private string _searchText = string.Empty;
        
        private static bool TourIsSelected { get; set; }

        public List<Tour> TourList
        {
            get => this._tourList;
            set
            {
                this._tourList = value;
                OnPropertyChanged();
            }
        }

        public string LogTimeFrom
        {
            get => this._log.TimeFrom;
            set
            {
                this._log.TimeFrom = value;
                OnPropertyChanged();
            }
        }

        public string LogTimeTo
        {
            get => this._log.TimeTo;
            set
            {
                this._log.TimeTo = value;
                OnPropertyChanged();
            }
        }

        public int LogDistance
        {
            get => this._log.Distance;
            set
            {
                this._log.Distance = value;
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
                Debug.Print("Search for tour '" + this.SearchText + "' was triggered.");
                this.TourList = this._tourService.FindTours(this.SearchText);
            }
        }

        public ICommand DeleteTourLogCommand { get; set; }

        public ICommand EditTourLogCommand { get; set; }

        public ICommand SaveTourLogCommand { get; set; }

        public ICommand CreateReportCommand { get; }

        public ICommand SearchToursCommand { get; }

        private void EditTourLog(object parameter)
        {
            Debug.Print("Edit tour was triggered.");
        }

        private void DeleteTourLog(object parameter)
        {
            Debug.Print("Delete tour was triggered.");
        }

        private void SaveTourLog(object paramter)
        {
            Debug.Print("Save tour was triggered.");
        }

        private bool SaveTourLogCanExecute(object parameter)
        {
            bool logTimeFromIsValid = !string.IsNullOrWhiteSpace(this.LogTimeFrom);
            bool logTimeToIsValid = !string.IsNullOrWhiteSpace(this.LogTimeTo);
            bool logDistanceIsValid = this.LogDistance >= 0;
            return logTimeFromIsValid && logTimeToIsValid && logDistanceIsValid;
        }

        private void CreateReport(object parameter)
        {
            Debug.Print("Create report was triggered.");
        }

        private void SearchTour(object parameter)
        {
            Debug.Print("Search for tour '" + this.SearchText + "' was triggered.");
            var tours = this._tourService.GetTours();

            if (string.IsNullOrWhiteSpace(this.SearchText))
            {
                this.TourList = tours;
                return;
            }

            this.TourList = tours.FindAll(tour => tour.From.Contains(this.SearchText) || tour.To.Contains(this.SearchText));
        }

        private bool SearchTourCanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(this.SearchText);
        }

        public HomeViewModel(ITourService tourService)
        {
            this._tourService = tourService;
            this.TourList = this._tourService.GetTours();
            TourIsSelected = false;

            this.EditTourLogCommand = new RelayCommand(EditTourLog, _ => TourIsSelected);
            this.SaveTourLogCommand = new RelayCommand(SaveTourLog, SaveTourLogCanExecute);
            this.DeleteTourLogCommand = new RelayCommand(DeleteTourLog, _ => TourIsSelected);
            this.CreateReportCommand = new RelayCommand(CreateReport);
            this.SearchToursCommand = new RelayCommand(SearchTour, SearchTourCanExecute);
        }
    }
}