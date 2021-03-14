using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        public List<string> Find(string query)
        {
            return new List<string>() { "Vienna", "New York" };
        }
    }
}
