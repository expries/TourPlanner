using System.Diagnostics;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.WPF.State;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.ViewModels
{
    public class NewTourViewModel : ViewModelBase
    {
        public string InputFrom 
        {
            get => this._tour.From;
            set
            {
                this._tour.From = value;
                OnPropertyChanged();
            }
        }

        public string InputTo
        {
            get => this._tour.To;
            set
            {
                this._tour.To = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchFromCommand { get; }

        public ICommand SearchToCommand { get; }
        
        public ICommand SaveTourCommand { get; }
        
        public ICommand LoadRouteCommand { get; }

        private IMapService MapService { get; }
        
        private ITourService TourService { get; }

        private bool RouteInfoWasLoaded { get; set; }
        
        private readonly Tour _tour = new Tour();

        public NewTourViewModel(IMapService mapService, ITourService tourService)
        {
            this.MapService = mapService;
            this.TourService = tourService;
            this.RouteInfoWasLoaded = false;

            this.SearchFromCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Search location (From) was triggered.");
                this.MapService.FindLocations(this.InputFrom);
            }, 
            parameter =>
            {
                return this.InputFrom.Trim().Length >= 2;
            });

            this.SearchToCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Search location (To) was triggered.");
                this.MapService.FindLocations(this.InputTo);
            },
            parameter =>
            {
                return this.InputTo.Trim().Length >= 2;
            });

            this.SaveTourCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Save tour was triggered.");
                tourService.SaveTour(this._tour);
            },
            parameter =>
            {
                return this.RouteInfoWasLoaded 
                       && this.InputTo.Trim().Length >= 2 
                       && this.InputFrom.Trim().Length >= 2;
            });

            this.LoadRouteCommand = new RelayCommand(parameter =>
            {
                Debug.Print("Load route was triggered,");
                this.RouteInfoWasLoaded = true;
            },
            paramter =>
            {
                return this.InputTo.Trim().Length >= 2 && this.InputFrom.Trim().Length >= 2;
            });
        }
    }
}