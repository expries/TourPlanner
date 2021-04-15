using System.Collections.Generic;

namespace TourPlanner.DAL.Repositories
{
    public interface ILocationRepository
    {
        public List<string> Find(string query);
    }
}
