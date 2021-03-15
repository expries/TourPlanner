using System.Windows.Input;
using TourPlanner.Factories;
using TourPlanner.ViewModels;

namespace TourPlanner.State
{
    public class Navigator : ViewModelBase, INavigator
    {
        public ViewModelBase CurrentViewModel { get; private set; }
        
        public ICommand UpdateCurrentViewModelCommand { get; }

        public Navigator(IViewModelFactory viewModelFactory)
        {
            CurrentViewModel = viewModelFactory.GetHomeViewModel();
            
            UpdateCurrentViewModelCommand = new RelayCommand(parameter =>
            {
                if (parameter is not ViewType viewType)
                {
                    return;
                }

                CurrentViewModel = viewType switch
                {
                    ViewType.Home  => viewModelFactory.GetHomeViewModel(),
                    ViewType.Route => viewModelFactory.GetRouteViewModel(),
                    _              => CurrentViewModel
                };

                OnPropertyChanged(nameof(CurrentViewModel));
            });
        }
    }
}