using System;

namespace TourPlanner.Domain.Models
{
    public class TourLog
    {
        [Column(Name="tourLogId")]
        public int TourLogId { get; set; }
        
        [Column(Name="startTime")]
        public DateTime TimeFrom { get; set; }

        [Column(Name="endTime")]
        public DateTime TimeTo { get; set; }

        [Column(Name="rating")] 
        public int Rating { get; set; }

        [ManyToOne(ForeignKey="fk_tourId", Table="tour", PrimaryKey="tourID")]
        public Lazy<Tour> Tour { get; set; }
    }
}
