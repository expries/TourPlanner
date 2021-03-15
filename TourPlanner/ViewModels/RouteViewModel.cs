using TourPlanner.Services;

namespace TourPlanner.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        private readonly ITourService _tourService;
        
        public RouteViewModel(ITourService tourService)
        {
            _tourService = tourService;
        }
    }
}