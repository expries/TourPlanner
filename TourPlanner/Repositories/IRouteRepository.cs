using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Repositories
{
    public interface IRouteRepository
    {
        public List<Route> GetRoutes();
    }
}