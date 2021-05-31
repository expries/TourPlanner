using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TourPlanner.DAL.Repositories
{
    public class RouteImageRepository : IRouteImageRepository
    {
        private readonly string _imageDirectory;

        public RouteImageRepository(IConfiguration configuration)
        {
            this._imageDirectory = configuration.GetSection("Paths")["ImageDirectory"];
        }

        public byte[] Get(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                return File.ReadAllBytes(imagePath);
            }

            return null;
        }

        public string Save(byte[] image)
        {
            string imagePath;
            
            do
            {
                imagePath = this._imageDirectory + "\\" + Guid.NewGuid() + ".png";
            } 
            while (File.Exists(imagePath));
            
            File.WriteAllBytes(imagePath, image);
            return imagePath;
        }

        public bool Delete(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
                return true;
            }

            return false;
        }
    }
}