using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Services
{
    public interface ILocationService
    {
        public List<string> Search(string query);
    }
}
