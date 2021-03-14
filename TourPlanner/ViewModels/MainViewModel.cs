using TourPlanner.State;

namespace TourPlanner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public INavigator Navigator { get; }
        
        public MainViewModel(INavigator navigator)
        {
            Navigator = navigator;
        }
    }
}