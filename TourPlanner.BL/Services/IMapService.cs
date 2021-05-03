using System.Collections.Generic;

namespace TourPlanner.BL.Services
{
    public interface IMapService
    {
        public List<string> FindLocations(string query);

        public byte[] GetImage(List<string> locations);
    }
}
