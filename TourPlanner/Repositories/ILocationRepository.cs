using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Repositories
{
    public interface ILocationRepository
    {
        public List<string> Find(string query);
    }
}
