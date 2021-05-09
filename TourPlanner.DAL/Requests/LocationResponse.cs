using System.Collections.Generic;
using Newtonsoft.Json;

namespace TourPlanner.DAL.Requests
{
    public class LocationResponse
    {
        [JsonProperty("collections")]
        public List<List<LocationData>> Collections { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
    }
}