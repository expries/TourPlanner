using System.Globalization;
using Newtonsoft.Json;

namespace TourPlanner.Domain.Models
{
    public class Coordinates
    {
        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        public override string ToString()
        {
            string lat = this.Lat.ToString(CultureInfo.InvariantCulture);
            string lng = this.Lng.ToString(CultureInfo.InvariantCulture);
            return lat + "," + lng;
        }
    }
}