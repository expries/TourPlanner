using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        public List<Route> GetRoutes()
        {
            return new();
        }
    }
}