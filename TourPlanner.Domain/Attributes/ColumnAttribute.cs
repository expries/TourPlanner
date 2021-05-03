using System;

namespace TourPlanner.Domain
{
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}