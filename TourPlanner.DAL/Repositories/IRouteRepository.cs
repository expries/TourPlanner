using TourPlanner.DAL.Requests;

namespace TourPlanner.DAL.Repositories
{
    public interface IRouteRepository
    {
        public DirectionResponse Get(string locationFrom, string locationTo);
        
        public LocationResponse FindLocation(string locationFrom, string locationTo);

        public byte[] GetImage(string sessionId, BoundingBox boundingBox, int width = 400, int height = 200);
    }
}