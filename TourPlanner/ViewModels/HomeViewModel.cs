using System.Windows.Input;
using TourPlanner.Services;
using TourPlanner.State;

namespace TourPlanner.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string _inputFrom = string.Empty;

        public string InputFrom 
        {
            get => _inputFrom;
            set {
                _inputFrom = value;
                OnPropertyChanged(nameof(InputFrom));
            }
        }

        private string _inputTo = string.Empty;

        public string InputTo
        {
            get => _inputTo;
            set
            {
                _inputTo = value;
                OnPropertyChanged(nameof(InputTo));
            }
        }

        public ICommand SearchFromCommand { get; }

        public ICommand SearchToCommand { get; }

        private ILocationService _locationService { get; }

        public HomeViewModel(ILocationService locationService)
        {
            _locationService = locationService;

            SearchFromCommand = new RelayCommand(parameter =>
            {
                _locationService.Search(InputFrom);
            }, 
            parameter =>
            {
                return InputFrom.Trim().Length >= 2;
            });

            SearchToCommand = new RelayCommand(parameter =>
            {
                _locationService.Search(InputTo);
            },
            parameter =>
            {
                return InputTo.Trim().Length >= 2;
            });
        }
    }
}