using System;
using Newtonsoft.Json;

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

        [JsonIgnore]
        [ManyToOne(ForeignKey="fk_tourId", Table="tour", PrimaryKey="tourID")]
        public Lazy<Tour> Tour { get; set; }

        public TourLog()
        {
        }

        public TourLog(Tour tour)
        {
            this.Tour = new Lazy<Tour>(() => tour);
        }

        public bool Equals(TourLog other)
        {
            return this.TourLogId == other.TourLogId && 
                   this.Date.Equals(other.Date) && 
                   this.Duration.Equals(other.Duration) && 
                   this.Distance.Equals(other.Distance) && 
                   this.Rating == other.Rating && 
                   this.Temperature.Equals(other.Temperature) && 
                   this.AverageSpeed.Equals(other.AverageSpeed) && 
                   this.DangerLevel == other.DangerLevel && 
                   this.Difficulty == other.Difficulty && 
                   this.Weather == other.Weather;
        }
    }
}
