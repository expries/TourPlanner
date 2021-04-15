﻿using System.Windows.Input;
using TourPlanner.ViewModels;

namespace TourPlanner.State
{
    public class Navigator : ViewModelBase, INavigator
    {
        public ViewModelBase CurrentViewModel { get; private set; }
        
        public ICommand UpdateCurrentViewModelCommand { get; }

        public Navigator(HomeViewModel homeViewModel, NewTourViewModel newTourViewModel)
        {
            CurrentViewModel = homeViewModel;
            
            UpdateCurrentViewModelCommand = new RelayCommand(parameter =>
            {
                if (parameter is not ViewType viewType)
                {
                    return;
                }

                CurrentViewModel = viewType switch
                {
                    ViewType.NewTour => newTourViewModel,
                    ViewType.Home    => homeViewModel,
                    _                => CurrentViewModel
                };

                OnPropertyChanged(nameof(CurrentViewModel));
            });
        }
    }
}