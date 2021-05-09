using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.DAL.Requests;

namespace TourPlanner.DAL.Repositories
{
    public interface IMapRepository
    {
        public LocationResponse FindLocation(string locationFrom, string locationTo);
        
        public DirectionResponse GetDirection(string locationFrom, string locationTo);

        public byte[] GetImage(string sessionId, BoundingBox boundingBox, int width = 400, int height = 200);
    }
}