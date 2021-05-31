using System;
using System.Collections.Generic;
using System.IO;

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
        
        [Column(Name="distance")]
        public double Distance { get; set; }

        [Column(Name="tourType")]
        public TourType Type { get; set; }

        [Column(Name="description")]
        public string Description { get; set; }

        [Column(Name="imagePath")]
        public string ImagePath { get; set; }

        [OneToMany(ForeignKey = "fk_tourID", Table = "tour_log")]
        public Lazy<List<TourLog>> TourLogs { get; set; }

        public byte[] Image
        {
            get
            {
                if (!File.Exists(this.ImagePath))
                {
                    return Array.Empty<byte>();
                }
                
                return File.ReadAllBytes(this.ImagePath);
            }
        }
        
        public Tour()
        {
            this.TourId = 0;
            this.Distance = 0;
            this.Name = string.Empty;
            this.From = string.Empty;
            this.To = string.Empty;
            this.Description = string.Empty;
            this.ImagePath = string.Empty;
            this.TourLogs = new Lazy<List<TourLog>>();
        }

        public bool Equals(Tour other)
        {
            return this.TourId == other.TourId &&
                   this.Name == other.Name &&
                   this.From == other.From &&
                   this.To == other.To &&
                   this.Distance.Equals(other.Distance) &&
                   this.Type == other.Type &&
                   this.Description == other.Description &&
                   this.ImagePath == other.ImagePath;
        }
    }
}