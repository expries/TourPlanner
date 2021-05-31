
namespace TourPlanner.DAL.Repositories
{
    public interface IRouteImageRepository
    {
        public byte[] Get(string imagePath);

        public string Save(byte[] image);

        public bool Delete(string imagePath);
    }
}