using Npgsql;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public class TourLogRepository : ITourLogRepository
    {
        private readonly IDatabaseConnection _connection;

        public TourLogRepository(IDatabaseConnection databaseConnection)
        {
            this._connection = databaseConnection;
        }
        
        public TourLog Get(int tourLogId)
        {
            try
            {
                const string sql = "SELECT * FROM tour_log WHERE tourLogId = @Id";
                return this._connection.QueryFirstOrDefault<TourLog>(sql, new { Id = tourLogId });   
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to get tour log with id " + tourLogId, ex);
            }
        }

        public TourLog Save(TourLog tourLog)
        {
            var savedTourLog = Get(tourLog.TourLogId);

            if (savedTourLog is null)
            {
                return Create(tourLog);
            }

            if (savedTourLog.Equals(tourLog))
            {
                return savedTourLog;
            }
            
            return Update(tourLog);
        }

        public bool Delete(TourLog tourLog)
        {
            try
            {
                const string sql = "DELETE FROM tour_log WHERE tourLogId = @Id";
                int result =  this._connection.Execute(sql, new { Id = tourLog.TourLogId });
                return result == 1;       
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to delete tour log", ex);
            }
        }
        
        public bool DeleteByTour(Tour tour)
        {
            try
            {
                const string sql = "DELETE FROM tour_log WHERE fk_tourid = @Id";
                int result =  this._connection.Execute(sql, new { Id = tour.TourId });
                return result > 0;       
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to delete tour logs by tour", ex);
            }
        }
        
        private TourLog Create(TourLog tourLog)
        {
            const string sql = "INSERT INTO tour_log (date, duration, distance, rating, temperature, averageSpeed, dangerLevel, difficulty, weather, fk_tourId) " +
                               "VALUES (@Date, @Duration, @Distance, @Rating, @Temperature, @AverageSpeed, @DangerLevel, @Difficulty, @Weather, @TourId) " +
                               "RETURNING tourLogId";

            try
            {
                object result = this._connection.ExecuteScalar(sql, new
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
                    TourId = tourLog.Tour.Value.TourId
                });

                tourLog.TourLogId = (int) result;
                return tourLog;   
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to create tour log", ex);
            }
        }
        
        private TourLog Update(TourLog tourLog)
        {
            const string sql = "UPDATE tour_log SET date = @Date, duration = @Duration, distance = @Distance, " +
                               "rating = @Rating, temperature = @Temperature, averageSpeed = @AverageSpeed, " +
                               "dangerLevel = @DangerLevel, difficulty = @Difficulty, weather = @Weather " +
                               "WHERE tourLogId = @Id";

            try
            {
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
                    Id = tourLog.TourLogId
                });

                return tourLog;
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to update tour log", ex);
            }
        }
    }
}