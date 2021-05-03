using System.Collections.Generic;
using Newtonsoft.Json;

namespace TourPlanner.DAL.Requests
{
    public class LocationResponse
    {
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
        
        [JsonProperty("request")]
        public Request Request { get; set; }
        
        [JsonProperty("results")]
        public List<LocationResult> Results { get; set; }
    }
    
    public class Pagination
    {
        [JsonProperty("nextUrl")]
        public string NextUrl { get; set; }
        
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }
    }

    public class Request
    {
        [JsonProperty("sort")]
        public string Sort { get; set; }
        
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("feedback")]
        public bool Feedback { get; set; }
        
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("q")]
        public string Q { get; set; }
    }
    
    public class LocationResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("displayString")]
        public string DisplayString { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("slug")]
        public string Slug { get; set; }
        
        [JsonProperty("language")]
        public string Language { get; set; }
        
        [JsonProperty("place")]
        public Place Place { get; set; }
    }
    
    public class Place
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
        
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Properties
    {
        [JsonProperty("city")]
        public string City { get; set; }
        
        [JsonProperty("stateCode")]
        public string StateCode { get; set; }
        
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        
        [JsonProperty("street")]
        public string Street { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}