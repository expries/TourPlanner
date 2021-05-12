using System;

namespace TourPlanner.Domain.Models
{
    public class TourLog
    {
        [Column(Name="tourLogId")]
        public int TourLogId { get; set; }
        
        [Column(Name="date")]
        public DateTime Date { get; set; }

        [Column(Name="duration")]
        public double Duration { get; set; }
        
        [Column(Name="distance")]
        public double Distance { get; set; }

        [Column(Name="rating")] 
        public int Rating { get; set; }
        
        [Column(Name="temperature")]
        public double Temperature { get; set; }
        
        [Column(Name="averageSpeed")]
        public double AverageSpeed { get; set; }
        
        [Column(Name="dangerLevel")]
        public int DangerLevel { get; set; }
        
        [Column(Name="difficulty")]
        public Difficulty Difficulty { get; set; }
        
        [Column(Name="weather")]
        public WeatherCondition Weather { get; set; }

        [ManyToOne(ForeignKey="fk_tourId", Table="tour", PrimaryKey="tourID")]
        public Lazy<Tour> Tour { get; set; }
    }
}
