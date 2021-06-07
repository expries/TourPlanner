using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TourPlanner.BL.Services;
using TourPlanner.Domain.Models;
using TourPlanner.WPF.State;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.Test
{
    [TestFixture]
    public class NewTourViewModelTests
    {
        private NewTourViewModel _model;
        
        private Mock<ITourService> _tourServiceMock;
        
        private Mock<IMapService> _mapServiceMock;
        
        private Mock<ITourListObservable> _tourListObeservableMock;

        [OneTimeSetUp]
        public void SetupNavigator()
        {
            var navigatorMock = new Mock<INavigator>();
            
            navigatorMock.Setup(navigator => navigator.UpdateCurrentViewModel(It.IsAny<ViewType>()));
            navigatorMock.Setup(navigator => navigator.UpdateCurrentViewModel(
                It.IsAny<ViewType>(), 
                It.IsAny<object>()));

            Navigator.Instance = navigatorMock.Object;
        }
        
        [SetUp]
        public void Setup()
        {
            this._tourServiceMock = new Mock<ITourService>();
            this._mapServiceMock = new Mock<IMapService>();
            this._tourListObeservableMock = new Mock<ITourListObservable>();

            var response = new List<List<Location>>();
            var taskResponse = new Task<List<List<Location>>>(() => response);

            this._mapServiceMock
                .Setup(service => service.FindLocationsAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>())
                )
                .Returns(taskResponse);
            
            this._model = new NewTourViewModel(
                this._mapServiceMock.Object, this._tourServiceMock.Object, this._tourListObeservableMock.Object);
        }
        
        [Test]
        public void Test_Validation_WhenAllFieldsAreSet_ValidationSucceeds()
        {
            // arrange + act
            this._model.FromQuery = "From";
            this._model.ToQuery = "To";
            this._model.From = "Location-From";
            this._model.To = "Location-To";
            this._model.TourName = "Tourname";

            // assert
            Assert.IsFalse(this._model.HasErrors);
        }

        [Test]
        public void Test_Validation_WhenTourDataIsSet_SaveTourCommandIsExecuted()
        {
            // arrange
            this._model.From = "Location-From";
            this._model.To = "Location-To";
            this._model.TourName = "Tourname";
            
            // act
            this._model.SaveTourCommand.Execute(null);
            
            // assert
            this._tourServiceMock.Verify(service => service.SaveTourAsync(It.IsAny<Tour>()), Times.Once);
        }
        
        [Test]
        public void Test_Validation_WhenTourDataIsEmpty_SaveTourCommandIsNotExecuted()
        {
            // arrange
            this._model.From = string.Empty;
            this._model.To = string.Empty;
            this._model.TourName = string.Empty;
            
            // act
            this._model.SaveTourCommand.Execute(null);
            
            // assert
            this._tourServiceMock.Verify(service => service.SaveTourAsync(It.IsAny<Tour>()), Times.Never);
        }
        
        [Test]
        public void Test_Validation_WhenTourFromOrToIsEmpty_ValidationFails()
        {
            // arrange
            this._model.From = string.Empty;
            this._model.To = string.Empty;
            
            // act
            var errorsFrom = this._model.GetErrors(nameof(this._model.From));
            var errorsTo = this._model.GetErrors(nameof(this._model.To));
            
            // assert
            Assert.IsTrue(this._model.HasErrors);
            Assert.AreEqual(1, GetCount(errorsFrom));
            Assert.AreEqual(1, GetCount(errorsTo));
        }
        
        [Test]
        public void Test_Validation_WhenTourFromOrToIsEmpty_LoadRouteCommandIsNotExecuted()
        {
            // arrange
            this._model.FromQuery = string.Empty;
            this._model.ToQuery = string.Empty;
            
            // act
            this._model.LoadRouteCommand.Execute(null);
            var fromErrors= this._model.GetErrors(nameof(this._model.FromQuery));
            var toErrors= this._model.GetErrors(nameof(this._model.ToQuery));
            
            // assert
            Assert.LessOrEqual(1, GetCount(fromErrors));
            Assert.LessOrEqual(1, GetCount(toErrors));
            
            this._mapServiceMock.Verify(service => service.FindLocationsAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>()
                ),
                Times.Never
            );
        }
        
        [Test]
        public void Test_Validation_WhenTourFromOrToIsSet_LoadRouteCommandIsExecuted()
        {
            // arrange
            this._model.FromQuery = "Vienna";
            this._model.ToQuery = "Berlin";
            
            // act
            this._model.LoadRouteCommand.Execute(null);
            
            // assert
            this._mapServiceMock.Verify(service => service.FindLocationsAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>()
                ),
                Times.Once
            );
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