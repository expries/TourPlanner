using System.Windows.Input;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF.State
{
    public class Navigator : ViewModelBase, INavigator
    {
        private ViewModelBase _currentViewModel;
        
        public ViewModelBase CurrentViewModel
        {
            get => this._currentViewModel;
            private set
            {
                var ctx = this._currentViewModel?.Context;
                this._currentViewModel = value;
                this._currentViewModel.OnNavigation(ctx);
                OnPropertyChanged(nameof(this.CurrentViewModel));
            }
        }
        
        public static INavigator Instance { get; private set; }

        public ICommand UpdateCurrentViewModelCommand { get; }

        public Navigator(HomeViewModel homeViewModel, NewTourViewModel newTourViewModel, NewTourLogViewModel newTourLogViewModel)
        {
            this.CurrentViewModel = homeViewModel;

            this.UpdateCurrentViewModelCommand = new RelayCommand(parameter =>
            {
                if (parameter is not ViewType viewType)
                {
                    return;
                }

                // copy the context to the new view model
                this.CurrentViewModel = viewType switch
                {
                    ViewType.NewTour    => newTourViewModel,
                    ViewType.Home       => homeViewModel,
                    ViewType.NewTourLog => newTourLogViewModel,
                    _                   => this.CurrentViewModel
                };
            });

            Instance = this;
        }
    }
}