using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;
        
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public MapService(IMapRepository mapRepository)
        {
            this._mapRepository = mapRepository;
        }
        
        public Task<List<List<Location>>> FindLocationsAsync(string from, string to)
        {
            return Task.Run(() => FindLocations(from, to));
        }

        public List<List<Location>> FindLocations(string from, string to)
        {
            try
            {
                Log.Debug($"Searching for locations from '{from}' to '{to}' ...");

                var result = new List<List<Location>>();
                var locationResponse = this._mapRepository.FindLocation(from, to);

                locationResponse.Collections?.ForEach(locationsSet =>
                {
                    var locations = locationsSet.FindAll(location =>
                        !string.IsNullOrEmpty(location.AdminArea5) &&
                        !string.IsNullOrEmpty(location.AdminArea3) &&
                        !string.IsNullOrEmpty(location.AdminArea1)
                    );

                    var uniqueNames = locations
                        .Select(location => location.AdminArea5 + ", " + location.AdminArea3 + ", " + location.AdminArea1)
                        .Distinct()
                        .ToList();

                    var uniqueLocations = uniqueNames.Select(name => 
                        locations.First(location =>
                        {
                            string locName = location.AdminArea5 + ", " + location.AdminArea3 + ", " + location.AdminArea1;
                            return locName.Equals(name);
                        })
                    ).ToList();

                    var locationsList = uniqueLocations.Select(location =>
                        new Location
                        {
                            Coordinates = location.Coordinates,
                            Name = location.AdminArea5,
                            FullName = location.AdminArea5 + ", " + location.AdminArea3 + ", " + location.AdminArea1
                        }
                    ).ToList();

                    result.Add(locationsList);
                });

                if (result.Count < 1)
                {
                    result.Add(new List<Location>());
                }

                if (result.Count < 2)
                {
                    result.Add(new List<Location>());
                }

                return result;
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to find locations", ex);
            }
        }
    }
}
