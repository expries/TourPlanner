using System.Diagnostics;
using System.Windows.Input;
using TourPlanner.Models;
using TourPlanner.Services;
using TourPlanner.State;

namespace TourPlanner.ViewModels
{
    public class NewTourViewModel : ViewModelBase
    {
        private readonly Tour _tour = new();

        public string InputFrom 
        {
            get => _tour.From;
            set
            {
                _tour.From = value;
                OnPropertyChanged();
            }
        }

        public string InputTo
        {
            get => _tour.To;
            set
            {
                _tour.To = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchFromCommand { get; }

        public ICommand SearchToCommand { get; }
        
        public ICommand SaveTourCommand { get; }
        
        public ICommand LoadRouteCommand { get; }

        private ILocationService LocationService { get; }
        
        private ITourService TourService { get; }

        private bool RouteInfoWasLoaded { get; set; }

        public NewTourViewModel(ILocationService locationService, ITourService tourService)
        {
            LocationService = locationService;
            TourService = tourService;
            RouteInfoWasLoaded = false;

            SearchFromCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Search location (From) was triggered.");
                LocationService.Search(InputFrom);
            }, 
            parameter =>
            {
                return InputFrom.Trim().Length >= 2;
            });

            SearchToCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Search location (To) was triggered.");
                LocationService.Search(InputTo);
            },
            parameter =>
            {
                return InputTo.Trim().Length >= 2;
            });

            SaveTourCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Save tour was triggered.");
                tourService.SaveTour(_tour);
            },
            parameter =>
            {
                return RouteInfoWasLoaded 
                && InputTo.Trim().Length >= 2 
                && InputFrom.Trim().Length >= 2;
            });

            LoadRouteCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Load route was triggered,");
                RouteInfoWasLoaded = true;
            },
            paramter =>
            {
                return InputTo.Trim().Length >= 2 && InputFrom.Trim().Length >= 2;
            });
        }
    }
}