using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TourPlanner.Domain;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly DatabaseConnection _connection;

        public TourRepository(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("default");
            this._connection = new DatabaseConnection(connectionString);
        }

        public Tour GetTour(int tourId)
        {
            const string sql = "SELECT * FROM tour WHERE tourId = @Id";
            return this._connection.QueryFirstOrDefault<Tour>(sql, new { Id = tourId });
        }

        public List<Tour> GetTours()
        {
            const string sql = "SELECT * FROM tour";
            return this._connection.Query<Tour>(sql);
        }

        public bool DeleteTour(Tour tour)
        {
            const string sql = "DELETE FROM tour WHERE tourId = @Id";
            int result =  this._connection.Execute(sql, new { Id = tour.TourId });
            return result == 1;
        }

        public Tour SaveTour(Tour tour)
        {
            var savedTour = GetTour(tour.TourId);

            if (savedTour is not null && savedTour.Equals(tour))
            {
                return savedTour;
            }

            if (savedTour is null)
            {
                return CreateTour(tour);
            }

            return UpdateTour(tour);
        }

        private Tour UpdateTour(Tour tour)
        {
            const string sql = "UPDATE tour SET name = @Name, locationFrom = @From, locationTo = @To, " +
                               "description = @Description, distance = @Distance, tourType = @TourType, " +
                               "imagePath = @ImagePath WHERE tourId = @Id";

            this._connection.Execute(sql, new
            {
                Name = tour.Name, 
                From = tour.From, 
                To = tour.To, 
                Description = tour.Description,
                Distance = tour.Distance,
                TourType = tour.Type,
                ImagePath = tour.ImagePath,
                Id = tour.TourId
            });

            var tourLogs = tour.TourLogs.Value;
            DeleteTourLogs(tour.TourId);
            tourLogs.ForEach(x => CreateTourLog(x, tour.TourId));
            return GetTour(tour.TourId);
        }
        
        private Tour CreateTour(Tour tour)
        { 
            const string sql = "INSERT INTO tour (name, locationFrom, locationTo, description, distance, tourType, imagePath) " +
                                "VALUES (@Name, @From, @To, @Description, @Distance, @TourType, @ImagePath)";

            this._connection.Execute(sql, new
            {
                Name = tour.Name, 
                From = tour.From, 
                To = tour.To, 
                Description = tour.Description,
                Distance = tour.Distance,
                TourType = tour.Type,
                ImagePath = tour.ImagePath
            });

            tour.TourId = GetLatestTourId();
            tour.TourLogs.Value.ForEach(x => CreateTourLog(x, tour.TourId));
            return GetTour(tour.TourId);
        }

        private TourLog CreateTourLog(TourLog tourLog, int tourId)
        {
            const string sql = "INSERT INTO tour_log (date, duration, distance, rating, temperature, averageSpeed, dangerLevel, difficulty, weather, fk_tourId) " +
                               "VALUES (@Date, @Duration, @Distance, @Rating, @Temperature, @AverageSpeed, @DangerLevel, @Difficulty, @Weather, @TourId)";

            this._connection.Execute(sql, new
            {
                Date = tourLog.Date,
                Duration = tourLog.Duration,
                Distance = tourLog.Distance,
                Rating = tourLog.Rating,
                Temperature = tourLog.Temperature,
                AverageSpeed = tourLog.AverageSpeed,
                DangerLevel = tourLog.DangerLevel,  
                Difficulty = tourLog.Difficulty,
                Weather = tourLog.Weather,
                TourId = tourId
            });

            tourLog.TourLogId = GetLatestTourLogId();
            return tourLog;
        }

        private bool DeleteTourLogs(int tourId)
        {
            const string sql = "DELETE FROM tour_log WHERE fk_tourid = @Id";
            int result =  this._connection.Execute(sql, new { Id = tourId });
            return result > 0;
        }

        private int GetLatestTourId()
        {
            const string sql = "SELECT tourId FROM tour ORDER BY tourId DESC LIMIT 1;";
            var tour = this._connection.QueryFirstOrDefault<Tour>(sql);
            return tour.TourId;
        }

        private int GetLatestTourLogId()
        {
            const string sql = "SELECT tourLogId FROM tour_log ORDER BY tourLogId DESC LIMIT 1;";
            var tour = this._connection.QueryFirstOrDefault<TourLog>(sql);
            return tour.TourLogId;
        }
        
        private TourLog GetTourLog(int tourLogId)
        {
            const string sql = "SELECT * FROM tour_log WHERE tourLogId = @Id";
            return this._connection.QueryFirstOrDefault<TourLog>(sql, new { Id = tourLogId });
        }
        
        private bool DeleteTourLog(TourLog tourLog)
        {
            const string sql = "DELETE FROM tour_log WHERE tourLogId = @Id";
            int result =  this._connection.Execute(sql, new { Id = tourLog.TourLogId });
            return result == 1;
        }
    }
}