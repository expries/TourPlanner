using System.Drawing;
using System.IO;

namespace TourPlanner.BL.Helpers
{
    public static class ImageHelper
    {
        public static Image CreateImage(byte[] imageData)
        {
            using var stream = new MemoryStream(imageData);
            return new Bitmap(stream);
        }
    }
}