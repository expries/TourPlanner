using System.Collections.Generic;
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
        
        public List<LocationResult> FindLocation(string query)
        {
            string url = $"{this._baseUrl}/search/v4/place?sort=relevance&feedback=false&key={this._apiKey}&q={query}";
            var locationResponse = HttpConnection.Get<LocationResponse>(url);
            var locations = locationResponse.Results;
            return locations;
        }

        public DirectionResponse GetDirection(string locationFrom, string locationTo)
        {
            string url = $"{this._baseUrl}/directions/v2/route?key={this._apiKey}";
            var request = new DirectionRequest();
            request.Locations = new List<string> { locationFrom, locationTo };
            var response = HttpConnection.Post<DirectionResponse>(url, request);
            return response;
        }

        public byte[] GetImage(List<string> locations, int width = 400, int height = 200)
        {
            string locationString = string.Join("||", locations);
            string url = $"{this._baseUrl}/staticmap/v5/map?locations={locationString}&size={width},{height}@2x&key={this._apiKey}";
            byte[] response = HttpConnection.GetBytes(url);
            return response;
        }
    }
}