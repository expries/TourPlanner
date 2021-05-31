using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using TourPlanner.Domain.Exceptions;

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
            try
            {
                if (!File.Exists(imagePath))
                {
                    return null;
                }
                
                return File.ReadAllBytes(imagePath);
            }
            catch (Exception ex) when (ex is ArgumentException or ArgumentNullException or PathTooLongException or
                DirectoryNotFoundException or IOException or UnauthorizedAccessException)
            {
                throw new DataAccessException($"Failed to read file from path '{imagePath}'", ex);
            }
        }

        public string Save(byte[] image)
        {
            string imagePath;
            
            do
            {
                imagePath = this._imageDirectory + "\\" + Guid.NewGuid() + ".png";
            } 
            while (File.Exists(imagePath));

            try
            {
                File.WriteAllBytes(imagePath, image);
                return imagePath;
            }
            catch (Exception ex) when (ex is ArgumentException or ArgumentNullException or PathTooLongException or 
                DirectoryNotFoundException or IOException or UnauthorizedAccessException)
            {
                throw new DataAccessException($"Failed to save image to path '{imagePath}'", ex);
            }
        }

        public bool Delete(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    return false;
                }
                
                File.Delete(imagePath);
                return true;
            }
            catch (Exception ex) when (ex is ArgumentException or ArgumentNullException or PathTooLongException or 
                DirectoryNotFoundException or IOException or UnauthorizedAccessException or NotSupportedException)
            {
                throw new DataAccessException($"Failed to delete image from path '{imagePath}'", ex);
            }
        }
    }
}