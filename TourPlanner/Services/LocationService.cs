using System.Collections.Generic;
using System.Diagnostics;
using TourPlanner.Repositories;

namespace TourPlanner.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        
        public List<string> Search(string query)
        {
            Debug.Print("Searching for location '" + query + "' ...");
            return _locationRepository.Find(query);
        }
    }
}
