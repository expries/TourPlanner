using System.Collections.Generic;
using Newtonsoft.Json;

namespace TourPlanner.DAL.Requests
{
    public class DirectionRequestOptions
    {
        [JsonProperty("avoids")]
        public List<object> Avoids { get; set; }

        [JsonProperty("avoidTimedConditions")]
        public bool AvoidTimedConditions { get; set; }

        [JsonProperty("doReverseGeocode")]
        public bool DoReverseGeocode { get; set; }

        [JsonProperty("shapeFormat")]
        public string ShapeFormat { get; set; }

        [JsonProperty("generalize")]
        public int Generalize { get; set; }

        [JsonProperty("routeType")]
        public string RouteType { get; set; }

        [JsonProperty("timeType")]
        public int TimeType { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("enhancedNarrative")]
        public bool EnhancedNarrative { get; set; }

        [JsonProperty("drivingStyle")]
        public int DrivingStyle { get; set; }

        [JsonProperty("highwayEfficiency")]
        public double HighwayEfficiency { get; set; }
    }

    public class DirectionRequest
    {
        [JsonProperty("locations")]
        public List<string> Locations { get; set; }

        [JsonProperty("options")]
        public DirectionRequestOptions Options { get; set; }
    }
}