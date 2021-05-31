using TourPlanner.DAL.Requests;

namespace TourPlanner.DAL.Repositories
{
    public interface IMapRepository
    {
        public LocationResponse FindLocation(string locationFrom, string locationTo);
        
        public DirectionResponse GetRoute(string locationFrom, string locationTo);

        public byte[] GetImage(string sessionId, BoundingBox boundingBox, int width = 400, int height = 200);
    }
}