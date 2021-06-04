using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using TourPlanner.BL.Services;
using TourPlanner.Domain.Models;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.Test
{
    [TestFixture]
    public class NewTourLogViewModelTests
    {
        private NewTourLogViewModel _model;

        private Mock<ITourService> _tourServiceMock;

        [SetUp]
        public void Setup()
        {
            this._tourServiceMock = new Mock<ITourService>();
            this._model = new NewTourLogViewModel(this._tourServiceMock.Object);
            var tour = new Tour();
            this._model.OnNavigation(tour);
        }

        [Test]
        public void Test_Validate_WhenAllFieldsAreSet_ValidationSucceeds()
        {
            // arrange
            this._model.Date = DateTime.Now;
            this._model.Difficulty = Difficulty.Easy;
            this._model.Distance = 10;
            this._model.Duration = 5;
            this._model.Rating = 5;
            this._model.Temperature = 20;
            this._model.Weather = WeatherCondition.Cloudy;
            this._model.AverageSpeed = 10;
            this._model.DangerLevel = 3;

            // act + assert
            Assert.IsFalse(this._model.HasErrors);
        }
        
        [Test]
        public void Test_Validation_WhenDistanceIsInvalid_PropertyHasErrors()
        {
            // arrange
            this._model.Distance = -10;

            // act
            var errors = this._model.GetErrors(nameof(this._model.Distance));
            
            // assert
            Assert.IsTrue(this._model.HasErrors);
            Assert.LessOrEqual(1, GetCount(errors));
        }
        
        [Test]
        public void Test_Validation_WhenLogDataIsSet_SaveTourLogCommandIsExecuted()
        {
            // arrange
            this._model.Date = DateTime.Now;
            this._model.Difficulty = Difficulty.Easy;
            this._model.Distance = 10;
            this._model.Duration = 5;
            this._model.Rating = 5;
            this._model.Temperature = 20;
            this._model.Weather = WeatherCondition.Cloudy;
            this._model.AverageSpeed = 10;
            this._model.DangerLevel = 3;

            // act
            this._model.SaveTourLogCommand.Execute(null);
            
            // assert
            this._tourServiceMock.Verify(service => service.SaveTourLogAsync(It.IsAny<TourLog>()), Times.Once);
        }
        
        [Test]
        public void Test_Validation_WhenLogDataIsInvalid_SaveTourLogCommandIsNotExecuted()
        {
            // arrange
            this._model.Distance = -10;

            // act
            this._model.SaveTourLogCommand.Execute(null);
            
            // assert
            this._tourServiceMock.Verify(service => service.SaveTourLogAsync(It.IsAny<TourLog>()), Times.Never);
            Assert.IsTrue(this._model.HasErrors);
        }
        
        private static int GetCount(IEnumerable enumerable)
        {
            int count = 0;
            var enumerator = enumerable.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                count++;
            }

            return count;
        }
    }
}