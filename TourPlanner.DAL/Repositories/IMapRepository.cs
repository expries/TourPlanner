using System.Collections.Generic;
using TourPlanner.DAL.Requests;

namespace TourPlanner.DAL.Repositories
{
    public interface IMapRepository
    {
        public List<LocationResult> FindLocation(string query);
        
        public DirectionResponse GetDirection(string locationFrom, string locationTo);

        public byte[] GetImage(List<string> locations, int width = 400, int height = 200);
    }
}