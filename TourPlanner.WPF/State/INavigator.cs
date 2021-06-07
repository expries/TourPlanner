using System.Windows.Input;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF.State
{
    public interface INavigator
    {
        public ViewModelBase CurrentViewModel { get; }

        public ICommand UpdateCurrentViewModelCommand { get; }
        
        public void UpdateCurrentViewModel(ViewType viewType);

        public void UpdateCurrentViewModel(ViewType viewType, object context);
    }
}