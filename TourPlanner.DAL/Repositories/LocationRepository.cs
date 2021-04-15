using System.Collections.Generic;

namespace TourPlanner.DAL.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        public List<string> Find(string query)
        {
            return new List<string> { "Vienna", "New York" };
        }
    }
}
