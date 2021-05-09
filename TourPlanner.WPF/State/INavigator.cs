using System.Windows.Input;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF.State
{
    public interface INavigator
    {
        public ViewModelBase CurrentViewModel { get; }

        ICommand UpdateCurrentViewModelCommand { get; }
    }
}