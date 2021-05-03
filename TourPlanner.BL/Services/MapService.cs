using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TourPlanner.DAL.Repositories;

namespace TourPlanner.BL.Services
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;

        public MapService(IMapRepository mapRepository)
        {
            this._mapRepository = mapRepository;
        }
        
        public List<string> FindLocations(string query)
        {
            Debug.Print("Searching for location '" + query + "' ...");
            var locations =  this._mapRepository.FindLocation(query);
            var locationNames = locations.Select(x => x.Name).ToList();
            return locationNames;
        }

        public byte[] GetImage(List<string> locations)
        {
            return this._mapRepository.GetImage(locations);
        }
    }
}
