using System;
using System.Collections.Generic;

namespace TourPlanner.Domain.Models
{
    public class Tour
    {
        [Column(Name="tourId")] 
        public int TourId { get; set; }
        
        [Column(Name="name")]
        public string Name { get; set; }
        
        [Column(Name="locationFrom")]
        public string From { get; set; }

        [Column(Name="locationTo")]
        public string To { get; set; }

        [Column(Name="description")]
        public string Description { get; set; }

        [OneToMany(ForeignKey="fk_tourID", Table="tour_log")]
        public Lazy<List<TourLog>> TourLogs { get; set; }

        public Tour()
        {
            this.TourLogs = new Lazy<List<TourLog>>();
        }
    }
}