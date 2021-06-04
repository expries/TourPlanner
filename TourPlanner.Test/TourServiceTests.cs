using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Requests;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;

namespace TourPlanner.Test
{
    public class TourServiceTests
    {
        private Mock<ITourRepository> _tourRepositoryMock;
        
        private Mock<ITourLogRepository> _tourLogRepositoryMock;
        
        private Mock<IRouteRepository> _routeRepositoryMock;

        private Mock<IRouteImageRepository> _routeImageRepositoryMock;

        private ITourService _tourService;

        [SetUp]
        public void Setup()
        {
            this._tourRepositoryMock = new Mock<ITourRepository>();
            this._tourLogRepositoryMock = new Mock<ITourLogRepository>();
            this._routeRepositoryMock = new Mock<IRouteRepository>();
            this._routeImageRepositoryMock = new Mock<IRouteImageRepository>();
            
            this._tourService = new TourService(
                this._tourRepositoryMock.Object, 
                this._tourLogRepositoryMock.Object, 
                this._routeRepositoryMock.Object, 
                this._routeImageRepositoryMock.Object
            );
        }

        [Test]
        public void Test_GetTours_WhenOneTourIsSaved_ReturnsOneTour()
        {
            // arrange
            var tour = new Tour {TourId = 1};
            var tourList = new List<Tour>{tour};
            
            this._tourRepositoryMock
                .Setup(repository => repository.GetAll())
                .Returns(tourList);

            // act
            var tours = this._tourService.GetTours();

            // assert
            Assert.Contains(tour, tours);
        }
        
        [Test]
        public void Test_FindTours_WhenTourMatches_ReturnThatTour()
        {
            // arrange
            var tourA = new Tour {TourId = 1, Name = "ABC"};
            var tourB = new Tour {TourId = 2, Name = "123"};
            var tourList = new List<Tour>{tourA, tourB};
            
            this._tourRepositoryMock
                .Setup(repository => repository.GetAll())
                .Returns(tourList);

            // act
            var tours = this._tourService.FindTours(tourA.Name);

            // assert
            Assert.Contains(tourA, tours);
            Assert.AreEqual(1, tours.Count);
        }
        
        [Test]
        public void Test_FindTours_WhenNoTourMatches_ReturnEmptyList()
        {
            // arrange
            var tourA = new Tour {TourId = 1, Name = "Tour A"};
            var tourB = new Tour {TourId = 2, Name = "Tour B"};
            var tourList = new List<Tour>{tourA, tourB};
            
            this._tourRepositoryMock
                .Setup(repository => repository.GetAll())
                .Returns(tourList);
            
            // act
            var tours = this._tourService.FindTours("Not Matching");

            // assert
            Assert.AreEqual(0, tours.Count);
        }
        
        [Test]
        public void Test_FindTours_WhenTourHasMatchingLog_ReturnThatTour()
        {
            // arrange
            var tourLogA = new TourLog {TourLogId = 1, Description = "TourLogA"}; 
            var tourLogB = new TourLog {TourLogId = 2, Description = "TourLogB"};
            
            var tourA = new Tour
            {
                TourId = 1, 
                Name = "Tour A", 
                TourLogs = new Lazy<List<TourLog>>(new List<TourLog>{tourLogA})
            };
            
            var tourB = new Tour
            {
                TourId = 2, 
                Name = "Tour B", 
                TourLogs = new Lazy<List<TourLog>>(new List<TourLog>{tourLogB})
            };
            
            var tourList = new List<Tour>{tourA, tourB};
            
            this._tourRepositoryMock
                .Setup(repository => repository.GetAll())
                .Returns(tourList);

            // act
            var tours = this._tourService.FindTours(tourLogA.Description);

            // assert
            Assert.Contains(tourA, tours);
            Assert.AreEqual(1, tours.Count);
        }

        [Test]
        public void Test_SaveTour_WhenOldImageExists_ImageIsDeleted()
        {
            // arrange
            SetupMocksForSaveTour();
            var tour = new Tour();
            
            this._tourRepositoryMock
                .Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns(tour);

            // act
            this._tourService.SaveTour(tour);
            
            // assert
            this._routeImageRepositoryMock
                .Verify(repository => repository.Delete(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void Test_SaveTour_WhenOldImageDoesNotExist_ImageIsNotDeleted()
        {
            // arrange
            SetupMocksForSaveTour();
            var tour = new Tour();
            
            this._tourRepositoryMock
                .Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns(null as Tour);

            // act
            this._tourService.SaveTour(tour);
            
            // assert
            this._routeImageRepositoryMock
                .Verify(repository => repository.Delete(It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public void Test_SaveTour_WhenRouteCouldNotBeFound_BusinessExceptionIsThrown()
        {
            // arrange
            SetupMocksForSaveTour();
            var tour = new Tour {From = "TestA", To="TestB"};
            
            var directionResponse = new DirectionResponse
            {
                Info = new Info
                {
                    Statuscode = 400,  // this is 0 on success, otherwise something else
                    Messages = new List<object> {"Something went wrong"}
                }, 
                Route = new Route()
            };

            this._routeRepositoryMock
                .Setup(repository => repository.Get(
                    It.IsAny<string>(),
                    It.IsAny<string>())
                )
                .Returns(directionResponse);

            // act + assert
            Assert.Throws<BusinessException>(() => this._tourService.SaveTour(tour));
        }
        
        [Test]
        public void Test_SaveTour_WhenDescriptionIsEmpty_DescriptionIsSetToPlacholder()
        {
            // arrange
            SetupMocksForSaveTour();
            var tour = new Tour();

            // act
            tour = this._tourService.SaveTour(tour);
            
            // assert
            Assert.AreEqual("Keine Beschreibung", tour.Description);
        }
        
        private void SetupMocksForSaveTour()
        {
            var directionResponse = new DirectionResponse
            {
                Info = new Info
                {
                    Statuscode = 0
                }, 
                Route = new Route()
            };

            this._tourRepositoryMock
                .Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns(null as Tour);

            this._routeImageRepositoryMock
                .Setup(repository => repository.Delete(It.IsAny<string>()))
                .Returns(true);

            this._routeRepositoryMock
                .Setup(repository => repository.Get(
                    It.IsAny<string>(), 
                    It.IsAny<string>())
                )
                .Returns(directionResponse);

            this._routeRepositoryMock
                .Setup(repository => repository.GetImage(
                    It.IsAny<string>(),
                    It.IsAny<BoundingBox>(), 
                    It.IsAny<int>(), 
                    It.IsAny<int>())
                )
                .Returns(Array.Empty<byte>());

            this._routeImageRepositoryMock
                .Setup(repository => repository.Save(It.IsAny<byte[]>()))
                .Returns(string.Empty);

            this._tourRepositoryMock
                .Setup(repository => repository.Save(It.IsAny<Tour>()))
                .Returns<Tour>(x => x);
        }
    }
}