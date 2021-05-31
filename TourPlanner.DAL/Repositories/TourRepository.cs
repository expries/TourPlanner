using System;
using System.Collections.Generic;
using Npgsql;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.DAL.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly IDatabaseConnection _connection;

        public TourRepository(IDatabaseConnection databaseConnection)
        {
            this._connection = databaseConnection;
        }
        
        public List<Tour> GetAll()
        {
            try
            {
                const string sql = "SELECT * FROM tour";
                return this._connection.Query<Tour>(sql);
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to get all tours", ex);
            }
        }
        
        public Tour Get(int tourId)
        {
            try
            {
                const string sql = "SELECT * FROM tour WHERE tourId = @Id";
                return this._connection.QueryFirstOrDefault<Tour>(sql, new { Id = tourId });
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to get tour with id " + tourId, ex);
            }
        }

        public Tour Save(Tour tour)
        {
            var savedTour = Get(tour.TourId);

            if (savedTour is null)
            {
                return Create(tour);
            }

            if (savedTour.Equals(tour))
            {
                return savedTour;
            }

            return Update(tour);
        }

        public bool Delete(Tour tour)
        {
            const string sql = "DELETE FROM tour WHERE tourId = @Id";
            
            try
            {
                int result =  this._connection.Execute(sql, new { Id = tour.TourId });
                return result == 1;
            } 
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to delete tour", ex);
            }
        }

        private Tour Create(Tour tour)
        { 
            const string sql = "INSERT INTO tour (name, locationFrom, locationTo, description, distance, tourType, imagePath) " +
                                "VALUES (@Name, @From, @To, @Description, @Distance, @TourType, @ImagePath) " +
                                "RETURNING tourId";

            try
            {
                object result = this._connection.ExecuteScalar(sql, new
                {
                    Name = tour.Name,
                    From = tour.From,
                    To = tour.To,
                    Description = tour.Description,
                    Distance = tour.Distance,
                    TourType = tour.Type,
                    ImagePath = tour.ImagePath
                });

                tour.TourId = (int) result;
                return tour;
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to create tour", ex);
            }
        }
        
        private Tour Update(Tour tour)
        {
            const string sql = "UPDATE tour SET name = @Name, locationFrom = @From, locationTo = @To, " +
                               "description = @Description, distance = @Distance, tourType = @TourType, " +
                               "imagePath = @ImagePath WHERE tourId = @Id";

            try
            {
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

                return tour;
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to update tour", ex);
            }
        }
    }
}