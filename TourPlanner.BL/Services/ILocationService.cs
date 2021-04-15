using System.Collections.Generic;

namespace TourPlanner.BL.Services
{
    public interface ILocationService
    {
        public List<string> Search(string query);
    }
}
