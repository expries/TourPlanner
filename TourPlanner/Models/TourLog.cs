using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models
{
    class TourLog
    {
        public string TimeFrom;

        public string TimeTo;

        public int Distance;

        public TourLog()
        {
            TimeFrom = string.Empty;
            TimeTo = string.Empty;
            Distance = 0;
        }
    }
}
