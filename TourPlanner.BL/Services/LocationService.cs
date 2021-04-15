using System.Collections.Generic;
using System.Diagnostics;
using TourPlanner.DAL.Repositories;

namespace TourPlanner.BL.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            this._locationRepository = locationRepository;
        }
        
        public List<string> Search(string query)
        {
            Debug.Print("Searching for location '" + query + "' ...");
            return this._locationRepository.Find(query);
        }
    }
}
