using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TourPlanner.DAL.Requests;

namespace TourPlanner.DAL.Repositories
{
    public class MapRepository : IMapRepository
    {
        private readonly string _baseUrl;

        private readonly string _apiKey;
        
        public MapRepository(IConfiguration configuration)
        {
            var api = configuration.GetSection("Api");
            this._apiKey = api.GetValue<string>("Key");
            this._baseUrl = api.GetValue<string>("baseUrl");
        }
        
        public LocationResponse FindLocation(string locationFrom, string locationTo)
        {
            string url = $"{this._baseUrl}/directions/v2/route?key={this._apiKey}&ambiguities=check";
            var request = new DirectionRequest();
            request.Locations = new List<string> { locationFrom, locationTo };
            var response = HttpConnection.Post<LocationResponse>(url, request);
            return response;
        }

        public DirectionResponse GetDirection(string locationFrom, string locationTo)
        {
            string url = $"{this._baseUrl}/directions/v2/route?key={this._apiKey}";
            var request = new DirectionRequest();
            request.Locations = new List<string> { locationFrom, locationTo };
            var response = HttpConnection.Post<DirectionResponse>(url, request);
            return response;
        }

        public byte[] GetImage(string sessionId, BoundingBox boundingBox, int width = 400, int height = 200)
        {
            string boundingBoxString = $"{boundingBox.Lr.ToString()},{boundingBox.Ul.ToString()}";
            string url = $"{this._baseUrl}/staticmap/v5/map?session={sessionId}&boundingBox={boundingBoxString}&size={width},{height}@2x&key={this._apiKey}";
            var response = HttpConnection.GetBytes(url);
            return response;
        }
    }
}