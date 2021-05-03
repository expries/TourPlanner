using System;

namespace TourPlanner.Domain
{
    public class OneToManyAttribute : Attribute
    {
        public string ForeignKey { get; set; }

        public string Table { get; set; }
    }
}