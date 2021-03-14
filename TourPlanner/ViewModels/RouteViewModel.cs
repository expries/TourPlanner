using TourPlanner.Services;

namespace TourPlanner.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        private readonly IRouteService _routeService;
        
        public RouteViewModel(IRouteService routeService)
        {
            _routeService = routeService;
        }
    }
}