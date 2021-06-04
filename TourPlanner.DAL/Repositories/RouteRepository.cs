using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TourPlanner.DAL.Requests;

namespace TourPlanner.DAL.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly string _baseUrl;

        private readonly string _apiKey;
        
        public RouteRepository(IConfiguration configuration)
        {
            var api = configuration.GetSection("Api");
            this._apiKey = api.GetValue<string>("Key");
            this._baseUrl = api.GetValue<string>("BaseUrl");
        }
        
        public LocationResponse FindLocation(string locationFrom, string locationTo)
        {
            string url = $"{this._baseUrl}/directions/v2/route?key={this._apiKey}&ambiguities=check";
            var request = new DirectionRequest();
            request.Locations = new List<string> { locationFrom, locationTo };
            var response = HttpConnection.Post<LocationResponse>(url, request);
            return response;
        }

        public DirectionResponse Get(string locationFrom, string locationTo)
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
            byte[] response = HttpConnection.GetBytes(url);
            return response;
        }
    }
}