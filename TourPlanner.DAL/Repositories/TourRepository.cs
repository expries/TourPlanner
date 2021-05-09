using System;
using System.Collections.Generic;
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

            if (savedTour is not null)
            {
                DeleteTour(tour);
            }

            return CreateTour(tour);
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

            var tourLogs = tour.TourLogs.Value.Select(CreateTourLog).ToList();
            tour.TourLogs = new Lazy<List<TourLog>>(tourLogs);
            tour.TourId = GetLatestId();
            
            return tour;
        }

        private int GetLatestId()
        {
            const string sql = "SELECT tourId FROM tour ORDER BY tourId DESC LIMIT 1;";
            var tour = this._connection.QueryFirstOrDefault<Tour>(sql);
            return tour.TourId;
        }

        private TourLog CreateTourLog(TourLog tourLog)
        {
            const string sql = "INSERT INTO tour_log (startTime, endTime, rating, fk_tourId) " +
                               "VALUES (@TimeStart, @TimeEnd, @Rating, @TourId)";

            this._connection.Execute(sql, new
            {
                TimeStart = tourLog.TimeFrom,
                TimeEnd = tourLog.TimeTo,
                Rating = tourLog.Rating,
                TourId = tourLog.Tour.Value.TourId
            });

            return tourLog;
        }
    }
}