using System.Diagnostics;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.WPF.State;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TourPlanner.Domain;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.ViewModels
{
    public class NewTourViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private IMapService MapService { get; }

        private ITourService TourService { get; }

        private Dictionary<string, Location> Locations { get; } = new Dictionary<string, Location>();

        private bool _locationLoadingInProgress = false;
        
        private bool _tourSavingInProgress = false;

        // backing fields

        private List<string> _fromSuggestions = new List<string>();
        
        private List<string> _toSuggestions = new List<string>();
        
        private List<TourType> _tourTypes = new List<TourType>
        {
            TourType.Bicycle,
            TourType.Fastest,
            TourType.Pedestrian,
            TourType.Shortest
        };

        private string _fromQuery = string.Empty;
        
        private string _toQuery = string.Empty;
        
        private string _from = string.Empty;

        private string _to = string.Empty;

        private string _tourName = string.Empty;

        private string _description = string.Empty;

        private TourType _type = TourType.Fastest;

        private int _tourId = 0;
        
        // public properties

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string FromQuery
        {
            get => this._fromQuery;
            set
            {
                this._fromQuery = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string ToQuery
        {
            get => this._toQuery;
            set
            {
                this._toQuery = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string From 
        {
            get => this._from;
            set
            {
                this._from = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string To
        {
            get => this._to;
            set
            {
                this._to = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public TourType Type
        {
            get => this._type;
            set
            {
                this._type = value;
                OnPropertyChanged();
            }
        }
        
        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string TourName
        {
            get => this._tourName;
            set
            {
                this._tourName = value;
                OnPropertyChanged();
            }
        }
        

        public string Description
        {
            get => this._description;
            set
            {
                this._description = value;
                OnPropertyChanged();
            }
        }

        public List<TourType> TourTypes
        {
            get => this._tourTypes;
            set
            {
                this._tourTypes = value;
                OnPropertyChanged();
            }
        }
        
        public List<string> FromSuggestions
        {
            get => this._fromSuggestions;
            set
            {
                this._fromSuggestions = value;
                OnPropertyChanged();
            }
        }
        
        public List<string> ToSuggestions
        {
            get => this._toSuggestions;
            set
            {
                this._toSuggestions = value;
                OnPropertyChanged();
            }
        }
        
        // public commands
        
        public ICommand SaveTourCommand { get; }
        
        public ICommand LoadRouteCommand { get; }

        public NewTourViewModel(IMapService mapService, ITourService tourService)
        {
            this.MapService = mapService;
            this.TourService = tourService;

            this.LoadRouteCommand = new RelayCommand(LoadRoute, LoadRouteCanExecute);
            this.SaveTourCommand = new RelayCommand(SaveTour, SaveTourCanExecute);
        }

        public override void OnNavigation(object context)
        {
            if (context is not Tour tour)
            {
                tour = new Tour();
            }

            LoadTour(tour);
        }

        private async void LoadRoute(object parameter)
        {
            Debug.Print("Load route was triggered,");

            this._locationLoadingInProgress = true;
            var locations = await this.MapService.FindLocationsAsync(this.FromQuery, this.ToQuery);
            this._locationLoadingInProgress = false;
            
            var locationsFrom = locations[0];
            var locationsTo = locations[1];

            this.FromSuggestions = locationsFrom.Select(x => x.FullName).ToList();
            this.ToSuggestions = locationsTo.Select(x => x.FullName).ToList();
            
            this.Locations.Clear();
            locationsFrom.ForEach(x => this.Locations[x.FullName] =  x);
            locationsTo.ForEach(x => this.Locations[x.FullName] = x);
        }

        private bool LoadRouteCanExecute(object parameter)
        {
            var dependentProperties = new List<string>
            {
                nameof(this.FromQuery), 
                nameof(this.ToQuery)
            };
            
            return dependentProperties.All(ValidateProperty) && !this._locationLoadingInProgress;
        }

        private async void SaveTour(object parameter)
        {
            Debug.Print("Save tour was triggered");

            var from = this.Locations[this.From];
            var to = this.Locations[this.To];
            
            var tour = new Tour();
            tour.TourId = this._tourId;
            tour.Name = this.TourName;
            tour.From = from.FullName;
            tour.To = to.FullName;
            tour.Description = this.Description;
            tour.Type = this.Type;

            this._tourSavingInProgress = true;
            var newTour = await this.TourService.SaveTourAsync(tour);
            this._tourSavingInProgress = false;
            this.Context = newTour;
            Navigator.Instance.UpdateCurrentViewModelCommand.Execute(ViewType.Home);
        }

        private bool SaveTourCanExecute(object parameter)
        {
            var dependentProperties = new List<string>
            {
                nameof(this.From), 
                nameof(this.To), 
                nameof(this.Type), 
                nameof(this.TourName)
            };
            
            return dependentProperties.All(ValidateProperty) && !this._tourSavingInProgress;
        }
        
        private void LoadTour(Tour tour)
        {
            this.Type = tour.Type;
            this.TourName = tour.Name;
            this.From = tour.From;
            this.To = tour.To;
            this.Description = tour.Description;
            this._tourId = tour.TourId;

            this._fromQuery = tour.From;
            this.ToQuery = tour.To;
            
            this.FromSuggestions = new List<string> { tour.From };
            this.ToSuggestions = new List<string> { tour.To };
            this.Locations[tour.From] = new Location { FullName = tour.From };
            this.Locations[tour.To] = new Location { FullName = tour.To };
        }
    }
}