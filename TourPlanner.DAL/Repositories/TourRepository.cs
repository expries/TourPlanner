using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
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

            if (savedTour.Equals(tour))
            {
                return savedTour;
            }

            DeleteTour(tour);
            return CreateTour(tour);
        }

        public Tour CreateTour(Tour tour)
        { 
            const string sql = "INSERT INTO tour (tourId, name, locationFrom, locationTo, description) " +
                                "VALUES (@Id, @Name, @From, @To, @Description)";

            this._connection.Execute(sql, new
            {
                Id = tour.TourId, 
                Name = tour.Name, 
                From = tour.From, 
                To = tour.To, 
                Description = tour.Description
            });

            var tourLogs = tour.TourLogs.Value.Select(CreateTourLog).ToList();
            tour.TourLogs = new Lazy<List<TourLog>>(tourLogs);
            return tour;
        }

        private TourLog CreateTourLog(TourLog tourLog)
        {
            const string sql = "INSERT INTO tour_log (tourLogId, startTime, endTime, rating, fk_tourId) " +
                               "VALUES (@Id, @TimeStart, @TimeEnd, @Rating, @TourId)";

            this._connection.Execute(sql, new
            {
                Id = tourLog.TourLogId,
                TimeStart = tourLog.TimeFrom,
                TimeEnd = tourLog.TimeTo,
                Rating = tourLog.Rating,
                TourId = tourLog.Tour.Value.TourId
            });

            return tourLog;
        }
    }
}