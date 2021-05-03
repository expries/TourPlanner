using System;

namespace TourPlanner.Domain
{
    public class ManyToOneAttribute : Attribute
    {
        public string ForeignKey { get; set; }

        public string Table { get; set; }

        public string PrimaryKey { get; set; }
    }
}