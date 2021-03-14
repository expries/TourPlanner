using TourPlanner.ViewModels;

namespace TourPlanner.Factories
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly HomeViewModel _homeViewModel;

        private readonly RouteViewModel _routeViewModel;
        
        public ViewModelFactory(HomeViewModel homeViewModel, RouteViewModel routeViewModel)
        {
            _homeViewModel = homeViewModel;
            _routeViewModel = routeViewModel;
        }

        public HomeViewModel GetHomeViewModel()
        {
            return _homeViewModel;
        }

        public RouteViewModel GetRouteViewModel()
        {
            return _routeViewModel;
        }
    }
}