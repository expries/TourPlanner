using TourPlanner.ViewModels;

namespace TourPlanner.Factories
{
    public interface IViewModelFactory
    {
        HomeViewModel GetHomeViewModel();
        
        RouteViewModel GetRouteViewModel();
    }
}