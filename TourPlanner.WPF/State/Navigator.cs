using System.Windows.Input;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF.State
{
    public class Navigator : ViewModelBase, INavigator
    {
        public ViewModelBase CurrentViewModel { get; private set; }
        
        public ICommand UpdateCurrentViewModelCommand { get; }

        public Navigator(HomeViewModel homeViewModel, NewTourViewModel newTourViewModel)
        {
            this.CurrentViewModel = homeViewModel;

            this.UpdateCurrentViewModelCommand = new RelayCommand(parameter =>
            {
                if (parameter is not ViewType viewType)
                {
                    return;
                }

                this.CurrentViewModel = viewType switch
                {
                    ViewType.NewTour => newTourViewModel,
                    ViewType.Home    => homeViewModel,
                    _                => this.CurrentViewModel
                };

                OnPropertyChanged(nameof(this.CurrentViewModel));
            });
        }
    }
}