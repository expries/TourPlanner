using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.WPF.State;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TourPlanner.Domain;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.ViewModels
{
    public class NewTourViewModel : ValidatedViewModelBase
    {
        private IMapService MapService { get; }

        private ITourService TourService { get; }

        private bool _locationLoadingInProgress;

        private bool _tourSaveInProgress;
        
        private bool _tourSaveTriggered;

        private bool _loadRouteTriggered;

        // backing fields

        private List<string> _fromSuggestions = new List<string>();
        
        private List<string> _toSuggestions = new List<string>();

        private string _fromQuery = string.Empty;
        
        private string _toQuery = string.Empty;

        private Tour _tour = new Tour();

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
            get => this._tour.From;
            set
            {
                this._tour.From = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string To
        {
            get => this._tour.To;
            set
            {
                this._tour.To = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public TourType Type
        {
            get => this._tour.Type;
            set
            {
                this._tour.Type = value;
                OnPropertyChanged();
            }
        }
        
        [Required(ErrorMessage = "Dieses Feld ist verpflichtend.")]
        public string TourName
        {
            get => this._tour.Name;
            set
            {
                this._tour.Name = value;
                OnPropertyChanged();
            }
        }
        
        public string Description
        {
            get => this._tour.Description;
            set
            {
                this._tour.Description = value;
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
        
        public List<TourType> TourTypes { get; } = new List<TourType>
        {
            TourType.Bicycle,
            TourType.Fastest,
            TourType.Pedestrian,
            TourType.Shortest
        };
        
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
            
            this.FromQuery = tour.From;
            this.ToQuery = tour.To;
            this.FromSuggestions = new List<string> { tour.From };
            this.ToSuggestions = new List<string> { tour.To };
            this._tour = tour;
            
            ClearValidationErrors();
        }

        private async void LoadRoute(object parameter)
        {
            try
            {
                this._loadRouteTriggered = true;

                if (!LoadRouteCanExecute(null))
                {
                    return;
                }

                this._locationLoadingInProgress = true;
                var locations = await this.MapService.FindLocationsAsync(this.FromQuery, this.ToQuery);
                this._locationLoadingInProgress = false;

                var locationsFrom = locations[0];
                var locationsTo = locations[1];

                this.FromSuggestions = locationsFrom.Select(x => x.FullName).ToList();
                this.ToSuggestions = locationsTo.Select(x => x.FullName).ToList();
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
                this._locationLoadingInProgress = false;
            }
        }

        private bool LoadRouteCanExecute(object parameter)
        {
            if (!this._loadRouteTriggered)
            {
                return true;
            }

            if (this._locationLoadingInProgress)
            {
                return false;
            }
            
            var dependentProperties = new List<string>
            {
                nameof(this.FromQuery), 
                nameof(this.ToQuery)
            };
            
            bool valid = dependentProperties.All(ValidateProperty);
            return valid && !this._locationLoadingInProgress;
        }

        private async void SaveTour(object parameter)
        {
            try
            {
                this._tourSaveTriggered = true;
                
                if (!SaveTourCanExecute(null))
                {
                    return;
                }

                this._tourSaveInProgress = true;
                var newTour = await this.TourService.SaveTourAsync(this._tour);
                this._tourSaveInProgress = false;
            
                this.Context = newTour;
                Navigator.Instance.UpdateCurrentViewModelCommand.Execute(ViewType.Home);
                this.Context = null;
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
                this._tourSaveInProgress = false;
            }
        }

        private bool SaveTourCanExecute(object parameter)
        {
            if (!this._tourSaveTriggered)
            {
                return true;
            }
            
            var dependentProperties = new List<string>
            {
                nameof(this.From), 
                nameof(this.To), 
                nameof(this.Type), 
                nameof(this.TourName)
            };

            bool valid = dependentProperties.All(ValidateProperty); 
            return valid && !this._tourSaveInProgress;
        }
    }
}