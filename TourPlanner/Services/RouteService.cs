using System.Collections.Generic;
using TourPlanner.Models;
using TourPlanner.Repositories;

namespace TourPlanner.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        
        public RouteService(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }
        
        public List<Route> GetRoutes()
        {
            return _routeRepository.GetRoutes();
        }
    }
}