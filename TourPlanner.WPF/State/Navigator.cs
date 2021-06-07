using System.Windows.Input;
using TourPlanner.Domain.Models;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF.State
{
    public class Navigator : ViewModelBase, INavigator
    {
        private ViewModelBase _currentViewModel;

        private readonly HomeViewModel _homeViewModel;

        private readonly NewTourViewModel _newTourViewModel;

        private readonly NewTourLogViewModel _newTourLogViewModel;
        
        public ViewModelBase CurrentViewModel
        {
            get => this._currentViewModel;
            private set
            {
                this._currentViewModel = value;
                OnPropertyChanged();
            }
        }
        
        public static INavigator Instance { get; set; }

        public ICommand UpdateCurrentViewModelCommand { get; }

        public Navigator(HomeViewModel homeViewModel, NewTourViewModel newTourViewModel, 
                         NewTourLogViewModel newTourLogViewModel)
        {
            this._homeViewModel = homeViewModel;
            this._newTourViewModel = newTourViewModel;
            this._newTourLogViewModel = newTourLogViewModel;

            Instance = this;
            this.CurrentViewModel = homeViewModel;
            this.UpdateCurrentViewModelCommand = new RelayCommand(UpdateCurrentViewModel);
        }

        public void UpdateCurrentViewModel(ViewType viewType)
        {
            this.CurrentViewModel = GetViewModel(viewType);
            this.CurrentViewModel.Accept();
        }
        
        public void UpdateCurrentViewModel(ViewType viewType, object context)
        {
            if (context is Tour tour)
            {
                UpdateCurrentViewModel(viewType, tour);
                return;
            }
            
            if (context is TourLog tourLog)
            {
                UpdateCurrentViewModel(viewType, tourLog);
                return;
            }
            
            this.CurrentViewModel = GetViewModel(viewType);
            this.CurrentViewModel.Accept(context);
        }

        public void UpdateCurrentViewModel(ViewType viewType, Tour tour)
        {
            dynamic viewModel = GetViewModel(viewType);
            this.CurrentViewModel = viewModel;
            viewModel.Accept(tour);
        }
        
        public void UpdateCurrentViewModel(ViewType viewType, TourLog tourLog)
        {
            dynamic viewModel = GetViewModel(viewType);
            this.CurrentViewModel = viewModel;
            viewModel.Accept(tourLog);
        }
        
        private void UpdateCurrentViewModel(object parameter)
        {
            if (parameter is ViewType viewType)
            {
                UpdateCurrentViewModel(viewType);
            }
        }
        
        private dynamic GetViewModel(ViewType viewType)
        {
            // copy the context to the new view model
            return viewType switch
            {
                ViewType.NewTour    => this._newTourViewModel,
                ViewType.Home       => this._homeViewModel,
                ViewType.NewTourLog => this._newTourLogViewModel,
                _                   => this.CurrentViewModel
            };
        }
    }
}