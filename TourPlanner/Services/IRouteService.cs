using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Services
{
    public interface IRouteService
    {
        public List<Route> GetRoutes();
    }
}