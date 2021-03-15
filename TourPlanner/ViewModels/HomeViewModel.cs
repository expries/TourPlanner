using System.Windows.Input;
using TourPlanner.Models;
using TourPlanner.Services;
using TourPlanner.State;

namespace TourPlanner.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly Tour _tour = new();

        public string InputFrom 
        {
            get => _tour.From;
            set => SetProperty(ref _tour.From, value);
        }

        public string InputTo
        {
            get => _tour.To; 
            set => SetProperty(ref _tour.To, value);
        }

        public string Description
        {
            get => _tour.Description;
            set => SetProperty(ref _tour.Description, value);
        }

        public ICommand SearchFromCommand { get; }

        public ICommand SearchToCommand { get; }
        
        public ICommand SaveTourCommand { get; }
        
        public ICommand LoadRoutePictureCommand { get; }

        private ILocationService LocationService { get; }
        
        private ITourService TourService { get; }

        public HomeViewModel(ILocationService locationService, ITourService tourService)
        {
            LocationService = locationService;
            TourService = tourService;

            SearchFromCommand = new RelayCommand(parameter =>
            {
                LocationService.Search(InputFrom);
            }, 
            parameter =>
            {
                return InputFrom.Trim().Length >= 2;
            });

            SearchToCommand = new RelayCommand(parameter =>
            {
                LocationService.Search(InputTo);
            },
            parameter =>
            {
                return InputTo.Trim().Length >= 2;
            });

            SaveTourCommand = new RelayCommand(parameter =>
            {
                tourService.SaveRoute(_tour);
            },
            parameter =>
            {
                return InputTo.Trim().Length >= 2 && InputFrom.Trim().Length >= 2 && Description.Trim().Length >= 2;
            });

            LoadRoutePictureCommand = new RelayCommand(parameter =>
            {
                var picture= tourService.LoadRoutePicture(_tour);
            },
            paramter =>
            {
                return InputTo.Trim().Length >= 2 && InputFrom.Trim().Length >= 2;
            });
        }
    }
}