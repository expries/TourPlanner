using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain.Models;

namespace TourPlannerTestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            var configuration = builder.Build();
            
            App.Start(configuration);
        }
    }

    public static class App
    {
        public static void Start(IConfiguration configuration)
        {
            //string connectionString = configuration.GetConnectionString("defaultConnectionString");
            //var dbConnection = new DatabaseConnection(connectionString);
            //const string sql = "SELECT * FROM tour";
            //var tours = dbConnection.Query<Tour>(sql);
            //var tour = tours.First().TourLogs.Value;
            
            //var x = new MapRepository(configuration);
            //var result = x.GetDirection("Graz", "Wien");
            //var locations = new List<string> { "Vienna|AT", "Graz|AT" };
            //byte[] image = x.GetImage(locations);
            Console.WriteLine("");
        }
    }
}