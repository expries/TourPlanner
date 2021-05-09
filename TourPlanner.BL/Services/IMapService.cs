using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.Domain.Models;

namespace TourPlanner.BL.Services
{
    public interface IMapService
    {
        public List<List<Location>> FindLocations(string from, string to);
        
        public Task<List<List<Location>>> FindLocationsAsync(string from, string to);
    }
}
