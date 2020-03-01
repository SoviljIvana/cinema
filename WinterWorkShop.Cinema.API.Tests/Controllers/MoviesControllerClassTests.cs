using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerClassTests
    {
        private Mock<IMovieService> _mockMoviesService;
        private Mock<IProjectionService> _mockProjectionsService;
        private Mock<ILogger<MoviesController>> _mockILogger;
        private MovieDomainModel _movieDomainModel;
        private Movie _movie;
        private List<MovieDomainModel> _listOfMovieDomainModels;

        [TestInitialize]
        public void TestInitialize()
        {
            _movieDomainModel = new MovieDomainModel()
            {
                Current = true,
                Id = Guid.NewGuid(),
                Rating = 9.5,
                Title = "New Title",
                Year = 2010
            };
            _listOfMovieDomainModels = new List<MovieDomainModel>();
            _listOfMovieDomainModels.Add(_movieDomainModel);

            _mockMoviesService = new Mock<IMovieService>();
            _mockProjectionsService = new Mock<IProjectionService>();
            _mockILogger = new Mock<ILogger<MoviesController>>();
        }

        [TestMethod]
        public void MoviesController_GetCurrentAndNotCurrent()
        {
            //Arrange
            IEnumerable<MovieDomainModel> movies = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movies);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentAndNotCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object);
            //Act
            var result = moviesController.GetCurrentAndNotCurrent().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var moviesDomainModelResultList = (List<MovieDomainModel>)resultList;
            //Assert
            Assert.IsNotNull(moviesDomainModelResultList);
            Assert.AreEqual(expectedResultCount, moviesDomainModelResultList.Count);
            Assert.AreEqual(_movieDomainModel.Id, moviesDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

    }
}
