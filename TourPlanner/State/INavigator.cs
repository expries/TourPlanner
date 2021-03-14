using System.Windows.Input;
using TourPlanner.ViewModels;

namespace TourPlanner.State
{
    public interface INavigator
    {
        public ViewModelBase CurrentViewModel { get; }
        
        ICommand UpdateCurrentViewModelCommand { get; }
    }
}