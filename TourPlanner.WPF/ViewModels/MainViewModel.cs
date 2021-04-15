using TourPlanner.WPF.State;

namespace TourPlanner.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public INavigator Navigator { get; }
        
        public MainViewModel(INavigator navigator)
        {
            this.Navigator = navigator;
        }
    }
}