using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Requests;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;

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
            Debug.Print("Searching for locations for from='" + from + "' to to=" + to + " ...");

            var result = new List<List<Location>>();
            var locations = this._mapRepository.FindLocation(from, to);

            locations.Collections?.ForEach(x =>
            {
                var validLocations = x.Where(y =>
                    !string.IsNullOrEmpty(y.AdminArea5) &&
                    !string.IsNullOrEmpty(y.AdminArea3) &&
                    !string.IsNullOrEmpty(y.AdminArea1)
                );
                
                var validLocationList = validLocations.ToList();
                
                var uniqueNames = validLocationList
                    .Select(y => y.AdminArea5 + ", " + y.AdminArea3 + ", " + y.AdminArea1)
                    .Distinct()
                    .ToList();

                var uniqueLocations = uniqueNames.Select(name => 
                    validLocationList.First(z =>
                    {
                        string locName = z.AdminArea5 + ", " + z.AdminArea3 + ", " + z.AdminArea1;
                        return locName.Equals(name);
                    })
                ).ToList();

                var locationsList = uniqueLocations.Select(l =>
                    new Location
                    {
                        Coordinates = l.Coordinates,
                        Name = l.AdminArea5,
                        FullName = l.AdminArea5 + ", " + l.AdminArea3 + ", " + l.AdminArea1
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
    }
}
