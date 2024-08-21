
using DreamTimeAPI;
using DreamTimeAPI.Controllers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DreamTeam.UnitTests
{
    public class TestWeatherForecastController
    {
        private Mock<ILogger<WeatherForecastController>> _logger = new();

        [Test]
        public void Should_Return_Weather_Forecast_For_Five_Days()
        {
            //Arrange
            var weatherForeCastController = new WeatherForecastController(_logger.Object);

            //Act 
            var result = weatherForeCastController.Get();

           //Assert
          result.Count()
                .Should()
                .Be(5);
          Assert.AreEqual(5, result.Count());
        }
        [Test]
        public void Should_Return_List_Of_WeatherForecast()
        {
            //Arrange
            var weatherForeCastController = new WeatherForecastController(_logger.Object);

            //Act 
            var result = weatherForeCastController.Get().ToList();

            //Assert
            result
                  .Should()
                  .BeOfType<List<WeatherForecast>>();
        }
    }
}
