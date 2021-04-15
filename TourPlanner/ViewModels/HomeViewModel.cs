﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using TourPlanner.Models;
using TourPlanner.Services;
using TourPlanner.State;

namespace TourPlanner.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private List<Tour> _tourList = new();

        private readonly TourLog _log = new();

        private string _searchText = string.Empty;
        
        private static bool TourIsSelected { get; set; }

        private readonly ITourService _tourService;

        public List<Tour> TourList
        {
            get => _tourList;
            set
            {
                _tourList = value;
                OnPropertyChanged();
            }
        }

        public string LogTimeFrom
        {
            get => _log.TimeFrom;
            set
            {
                _log.TimeFrom = value;
                OnPropertyChanged();
            }
        }

        public string LogTimeTo
        {
            get => _log.TimeTo;
            set
            {
                _log.TimeTo = value;
                OnPropertyChanged();
            }
        }

        public int LogDistance
        {
            get => _log.Distance;
            set
            {
                _log.Distance = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Debug.Print("Search for tour '" + SearchText + "' was triggered.");
                TourList = _tourService.FindTours(SearchText);
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
            bool logTimeFromIsValid = !string.IsNullOrWhiteSpace(LogTimeFrom);
            bool logTimeToIsValid = !string.IsNullOrWhiteSpace(LogTimeTo);
            bool logDistanceIsValid = LogDistance >= 0;
            return logTimeFromIsValid && logTimeToIsValid && logDistanceIsValid;
        }

        private void CreateReport(object parameter)
        {
            Debug.Print("Create report was triggered.");
        }

        private void SearchTour(object parameter)
        {
            Debug.Print("Search for tour '" + SearchText + "' was triggered.");
            var tours = _tourService.GetTours();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                TourList = tours;
                return;
            }

            TourList = tours.FindAll(tour => tour.From.Contains(SearchText) || tour.To.Contains(SearchText));
        }

        private bool SearchTourCanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SearchText);
        }

        public HomeViewModel(ITourService tourService)
        {
            _tourService = tourService;
            TourList = _tourService.GetTours();
            TourIsSelected = false;

            EditTourLogCommand = new RelayCommand(EditTourLog, _ => TourIsSelected);
            SaveTourLogCommand = new RelayCommand(SaveTourLog, SaveTourLogCanExecute);
            DeleteTourLogCommand = new RelayCommand(DeleteTourLog, _ => TourIsSelected);
            CreateReportCommand = new RelayCommand(CreateReport);
            SearchToursCommand = new RelayCommand(SearchTour, SearchTourCanExecute);
        }
    }
}