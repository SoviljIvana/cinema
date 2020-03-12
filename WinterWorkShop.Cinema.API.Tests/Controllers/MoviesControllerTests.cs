using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _mockMoviesService;
        private Mock<IProjectionService> _mockProjectionsService;
        private Mock<ITagService> _mockTagService;
        private Mock<ISeatService> _mockSeatService;
        private Mock<ILogger<MoviesController>> _mockILogger;
        private MovieDomainModel _movieDomainModel;
        private Movie _movie;
        private DeleteMovieModel _deleteMovieModel;
        private TagDomainModel _tagDomainModel;
        private MovieCreateTagDomainModel _movieCreateTagDomainModel;
        private CreateMovieModel _createMovieModel;
        private UpdateMovieModel _updateMovieModel;
        private ProjectionDomainModel _projectionDomainModel;
        private List<MovieDomainModel> _listOfMovieDomainModels;
        private List<ProjectionDomainModel> _listOfProjectionDomainModels;

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
            _projectionDomainModel = new ProjectionDomainModel()
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                MovieId = _movieDomainModel.Id,
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            _createMovieModel = new CreateMovieModel()
            {
                Rating = 2,
                Title = "MovieTitle",
                Year = 2015,
                Current = false
            };
            _movieCreateTagDomainModel = new MovieCreateTagDomainModel()
            {
                Duration = 0,
                tagsForMovieToAdd = new List<string>()
            };
            _updateMovieModel = new UpdateMovieModel()
            {
                Current = true,
                Rating = 9.5,
                Title = "New Title",
                Year = 2010
            };
            _tagDomainModel = new TagDomainModel()
            {
                actores = new List<TagForMovieDomainModel>(),
                awords = new List<TagForMovieDomainModel>(),
                genres = new List<TagForMovieDomainModel>(),
                languages = new List<TagForMovieDomainModel>(),
                states = new List<TagForMovieDomainModel>()
            };
            _listOfMovieDomainModels = new List<MovieDomainModel>();
            _listOfMovieDomainModels.Add(_movieDomainModel);
            _listOfProjectionDomainModels = new List<ProjectionDomainModel>();
            _listOfProjectionDomainModels.Add(_projectionDomainModel);

            _mockMoviesService = new Mock<IMovieService>();
            _mockProjectionsService = new Mock<IProjectionService>();
            _mockILogger = new Mock<ILogger<MoviesController>>();
            _mockTagService = new Mock<ITagService>();
            _mockSeatService = new Mock<ISeatService>();
        }

        [TestMethod]
        public void MoviesController_GetCurrentAndNotCurrent_ReturnOkObjectResult()
        {
            //Arrange
            IEnumerable<MovieDomainModel> movies = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movies);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentAndNotCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
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

        [TestMethod]
        public void MoviesController_GetCurrentAndNotCurrent_ReturnNotFoundObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            var errorMessageExpected = "Error occured while getting all movies, please try again.";
            IEnumerable<MovieDomainModel> movies = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movies);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentAndNotCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var result = moviesController.GetCurrentAndNotCurrent().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((NotFoundObjectResult)result).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(resultList, errorMessageExpected);
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void MoviesController_GetAsync_ReturnNotFoundObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            var errorMessageExpected = "Movie does not exist.";
            var id = Guid.NewGuid();
            MovieDomainModel movie = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(movie);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(id)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(result, errorMessageExpected);
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void MoviesController_GetAsync_ReturnOkObjectResult()
        {
            //Arrange
            int expectedStatusCode = 200;
            var id = Guid.NewGuid();
            MovieDomainModel movie = _movieDomainModel;
            Task<MovieDomainModel> responseTask = Task.FromResult(movie);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(id)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var movieDomainModel = (MovieDomainModel)result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(movie.Rating, movieDomainModel.Rating);
        }

        [TestMethod]
        public void MoviesController_GetProjectionsForSpecificMovie_ReturnOkObjectResult()
        {
            //Arrange
            int expectedStatusCode = 200;
            var id = Guid.NewGuid();
            IEnumerable<ProjectionDomainModel> projectionDomainModels = _listOfProjectionDomainModels;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            _mockProjectionsService.Setup(x => x.GetAllAsyncForSpecificMovie(id)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetProjectionsForSpecificMovie(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var listOfProjectionDomainModel = (List<ProjectionDomainModel>)result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(_projectionDomainModel.Id, listOfProjectionDomainModel[0].Id);
        }
        [TestMethod]
        public void MoviesController_GetProjectionsForSpecificMovie_ReturnNotFoundObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            var id = Guid.NewGuid();
            var errorMessageExpected = "Search returned with no results. Please try with different search parameter. ";

            IEnumerable<ProjectionDomainModel> projectionDomainModels = null;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            _mockProjectionsService.Setup(x => x.GetAllAsyncForSpecificMovie(id)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetProjectionsForSpecificMovie(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
            Assert.AreEqual(errorMessageExpected, result);
        }
        [TestMethod]
        public void MoviesController_SearchByTag_ReturnOkObjectResult()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            string searchData = "string";
            IEnumerable<MovieDomainModel> movieDomainModels = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetAllMoviesWithThisTag(searchData)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.SearchByTag(searchData).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var listOfMovieDomainModels = (List<MovieDomainModel>)result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(_movieDomainModel.Id, listOfMovieDomainModels[0].Id);
            Assert.AreEqual(expectedCount, listOfMovieDomainModels.Count);
        }
        [TestMethod]
        public void MoviesController_SearchByTag_ReturnStatusCodeObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            string searchData = "string";
            string expectedErrorMessage = "There is not movie that match this description, try something new.";
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetAllMoviesWithThisTag(searchData)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.SearchByTag(searchData).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
            Assert.IsInstanceOfType(result, typeof(ErrorResponseModel));
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
        }

        [TestMethod]
        public void MoviesController_GetCurrentWithProjections_ReturnOkObjectResult()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            IEnumerable<MovieDomainModel> movieDomainModels = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrentWithProjections().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var resultList = ((List<MovieDomainModel>)result);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedCount, resultList.Count);
            Assert.AreEqual(_movieDomainModel.Id, resultList[0].Id);
        }

        [TestMethod]
        public void MoviesController_GetCurrentWithProjections_ReturnStatusCodeObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie does not exist.";

            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrentWithProjections().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
        }

        [TestMethod]
        public void MoviesController_GetCurrentWithProjections_ReturnBadRequestObjectResult()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMovies()).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrentWithProjections().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_CommingSoon_ReturnOkObjectResult()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            IEnumerable<MovieDomainModel> movieDomainModels = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMoviesWithoutProjections()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.CommingSoon().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var resultList = ((List<MovieDomainModel>)result);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedCount, resultList.Count);
            Assert.AreEqual(_movieDomainModel.Id, resultList[0].Id);
        }

        [TestMethod]
        public void MoviesController_CommingSoon_ReturnStatusCodeObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie does not exist.";

            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMoviesWithoutProjections()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.CommingSoon().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
        }

        [TestMethod]
        public void MoviesController_CommingSoon_ReturnBadRequestObjectResult()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMoviesWithoutProjections()).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.CommingSoon().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void MoviesController_GetCurrentWithProjectionsForToday_ReturnOkObjectResult()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            IEnumerable<MovieDomainModel> movieDomainModels = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMoviesForToday()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrentWithProjectionsForToday().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var resultList = ((List<MovieDomainModel>)result);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedCount, resultList.Count);
            Assert.AreEqual(_movieDomainModel.Id, resultList[0].Id);
        }

        [TestMethod]
        public void MoviesController_GetCurrentWithProjectionsForToday_ReturnStatusCodeObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie does not exist.";

            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMoviesForToday()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrentWithProjectionsForToday().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
        }

        [TestMethod]
        public void MoviesController_GetCurrentWithProjectionsForToday_ReturnBadRequestObjectResult()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMoviesForToday()).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrentWithProjectionsForToday().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_CreateNewMovieWithHisTags_Returns_BadRequest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            CreateMovieModel createMovieModel = _createMovieModel;
            _mockMoviesService = new Mock<IMovieService>();
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            moviesController.ModelState.AddModelError("key", "Invalid Model State");
            //Act
            var result = moviesController.CreateNewMovieWithHisTags(createMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_CreateNewMovieWithHisTags_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";

            CreateMovieModel createMovieModel = _createMovieModel;
            MovieCreateTagDomainModel movieCreateTagDomainModel = _movieCreateTagDomainModel;

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>(), It.IsAny<MovieCreateTagDomainModel>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.CreateNewMovieWithHisTags(createMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_CreateNewMovieWithHisTags_Returns_StatusCode()
        {
            //Arrange
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Error occured while creating new movie, please try again.";

            CreateMovieModel createMovieModel = _createMovieModel;
            MovieCreateTagDomainModel movieCreateTagDomainModel = _movieCreateTagDomainModel;
            MovieDomainModel movieDomainModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>(), It.IsAny<MovieCreateTagDomainModel>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.CreateNewMovieWithHisTags(createMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;
            var resultStatusCode = ((ErrorResponseModel)result).StatusCode;
            var resultStatusCodeIntoInt = ((int)resultStatusCode);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }

        [TestMethod]
        public void MoviesController_CreateNewMovieWithHisTags_Returns_CreatedMovieDomainModel()
        {
            //Arrange
            int expectedStatusCode = 201;
            CreateMovieModel createMovieModel = _createMovieModel;
            MovieCreateTagDomainModel movieCreateTagDomainModel = _movieCreateTagDomainModel;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>(), It.IsAny<MovieCreateTagDomainModel>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.CreateNewMovieWithHisTags(createMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var createdResult = ((CreatedResult)resultAction).Value;
            var resultMovieDomainModel = (MovieDomainModel)createdResult;
            //Assert
            Assert.IsNotNull(resultMovieDomainModel);
            Assert.AreEqual(movieDomainModel.Rating, resultMovieDomainModel.Rating);
            Assert.IsInstanceOfType(resultAction, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)resultAction).StatusCode);
        }


        [TestMethod]
        public void MoviesController_UpdateMovie_Returns_BadRequeest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _mockMoviesService = new Mock<IMovieService>();
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            moviesController.ModelState.AddModelError("key", "Invalid Model State");
            //Act
            var result = moviesController.UpdateMovie(It.IsAny<Guid>(), It.IsAny<UpdateMovieModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_UpdateMovie_Returns_NotFound()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie does not exist.";

            MovieDomainModel movieDomainModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovie(It.IsAny<Guid>(), It.IsAny<UpdateMovieModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;
            var resultStatusCode = ((ErrorResponseModel)result).StatusCode;
            var resultStatusCodeIntoInt = ((int)resultStatusCode);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }

        [TestMethod]
        public void MoviesController_UpdateMovie_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            UpdateMovieModel updateMovieModel = _updateMovieModel;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovie(It.IsAny<Guid>(), updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void MoviesController_UpdateMovie_IsSuccessful_False_ProjectionInFuture_Return_BadRequest()
        {
            //Arrange
            UpdateMovieModel updateMovieModel = _updateMovieModel;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            int statusCodeExpected = 500;
            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Error occured while updating current movie status. This movie has projection in future, so it can not be not current."
            };
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            Task<CreateMovieResultModel> responseTaskUpdateMovie = Task.FromResult(createMovieResultModel);
            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(responseTaskUpdateMovie);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovie(It.IsAny<Guid>(), updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(createMovieResultModel.ErrorMessage, resultErrorResponseModel);
            Assert.AreEqual(statusCodeExpected, resultStatusCodeIntoInt);
        }
        [TestMethod]
        public void MoviesController_UpdateMovie_IsSuccessful_False_UpdateError_Return_BadRequest()
        {
            //Arrange
            UpdateMovieModel updateMovieModel = _updateMovieModel;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            int statusCodeExpected = 500;
            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Error occured while updating current movie status, please try again."
            };
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            Task<CreateMovieResultModel> responseTaskUpdateMovie = Task.FromResult(createMovieResultModel);
            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(responseTaskUpdateMovie);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovie(It.IsAny<Guid>(), updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(createMovieResultModel.ErrorMessage, resultErrorResponseModel);
            Assert.AreEqual(statusCodeExpected, resultStatusCodeIntoInt);
        }

        [TestMethod]
        public void MoviesController_UpdateMovie_Accepted()
        {
            //Arrange
            var expectedStatusCode = 202;
            UpdateMovieModel updateMovieModel = _updateMovieModel;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel()
            {
                Movie = _movieDomainModel,
                IsSuccessful = true,
                ErrorMessage = null
            };
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            Task<CreateMovieResultModel> responseTaskUpdateMovie = Task.FromResult(createMovieResultModel);
            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(responseTaskUpdateMovie);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovie(It.IsAny<Guid>(), updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var resultMovieDomainModel = (CreateMovieResultModel)resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultMovieDomainModel.IsSuccessful);
            Assert.IsNull(resultMovieDomainModel.ErrorMessage);
            Assert.AreEqual(updateMovieModel.Year, resultMovieDomainModel.Movie.Year);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }

        [TestMethod]
        public void MoviesController_DeleteMovie_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            string expectedErrorMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_DeleteMovie_Returns_StatusCode_MovieDoesNotExist()
        {
            //Arrange
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Movie does not exist.";
            DeleteMovieModel movieDomainModel = null;
            Task<DeleteMovieModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }
        [TestMethod]
        public void MoviesController_DeleteMovie_Returns_StatusCode_ProjectionInFuture()
        {
            //Arrange
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Cannot delete projection as it is scheduled in the future. ";
            DeleteMovieModel movieDomainModel = new DeleteMovieModel()
            {
                MovieDomainModel = null
            };
            Task<DeleteMovieModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }

        [TestMethod]
        public void MoviesController_DeleteMovie_Returns_Accepted()
        {
            //Arrange
            var expectedStatusCode = 202;
            DeleteMovieModel movieDomainModel = new DeleteMovieModel()
            {
                MovieDomainModel = _movieDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<DeleteMovieModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var resultMovieDomainModel = (DeleteMovieModel)resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultMovieDomainModel.IsSuccessful);
            Assert.IsNull(resultMovieDomainModel.ErrorMessage);
            Assert.AreEqual(_movieDomainModel.Year, resultMovieDomainModel.MovieDomainModel.Year);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }
        [TestMethod]
        public void MoviesController_GetAllTagsForMovieCreate_Returns_Tags()
        {
            //Arrange
            var expectedStatusCode = 200;
            TagDomainModel tagDomainModel = _tagDomainModel;
            Task<TagDomainModel> responseTask = Task.FromResult(tagDomainModel);
            _mockTagService = new Mock<ITagService>();
            _mockTagService.Setup(x => x.GetAllTags()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetAllTagsForMovieCreate().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var model = (TagDomainModel)resultList;
            //Assert
            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void MoviesController__GetAllTagsForMovieCreate_Returns_NotFoundObjectResult()
        {
            //Arrange
            var expectedStatusCode = 404;
            var expectedMessage = "Error occured while finding tags, please try again.";
            TagDomainModel tagDomainModel = null;
            Task<TagDomainModel> responseTask = Task.FromResult(tagDomainModel);
            _mockTagService = new Mock<ITagService>();
            _mockTagService.Setup(x => x.GetAllTags()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetAllTagsForMovieCreate().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void MoviesController_GetTopList_Returns_OkObjectResult()
        {
            //Arrange
            var expectedStatusCode = 200;
            var expectedResultCount = 1;
            IEnumerable<MovieDomainModel> movieDomainModels = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetTopTenMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetTopList().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void MoviesController_GetTopList_Returns_NotFoundObjectResult()
        {
            var expectedStatusCode = 404;
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetTopTenMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetTopList().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
        }


        [TestMethod]
        public void MoviesController_UpdateMovieCurrentStatus_Returns_NotFound()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie does not exist.";

            MovieDomainModel movieDomainModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovieCurrentStatus(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;
            var resultStatusCode = ((ErrorResponseModel)result).StatusCode;
            var resultStatusCodeIntoInt = ((int)resultStatusCode);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }

        [TestMethod]
        public void MoviesController_UpdateMovieCurrentStatus_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            MovieDomainModel movieDomainModel = _movieDomainModel;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService.Setup(x => x.UpdateMovieStatus(It.IsAny<Guid>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovieCurrentStatus(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void MoviesController_UpdateMovieCurrentStatus_IsSuccessful_False_ProjectionInFuture_Return_InternalServerError()
        {
            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            int statusCodeExpected = 500;
            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Error occured while updating current movie status. This movie has projection in future, so it can not be not current."
            };
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            Task<CreateMovieResultModel> responseTaskUpdateMovie = Task.FromResult(createMovieResultModel);
            _mockMoviesService.Setup(x => x.UpdateMovieStatus(It.IsAny<Guid>())).Returns(responseTaskUpdateMovie);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovieCurrentStatus(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(createMovieResultModel.ErrorMessage, resultErrorResponseModel);
            Assert.AreEqual(statusCodeExpected, resultStatusCodeIntoInt);
        }


        [TestMethod]
        public void MoviesController_UpdateMovieCurrentStatus_IsSuccessful_False_ErrorOccuredWhileUpdating_InternalServerError()
        {
            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            int statusCodeExpected = 500;
            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Error occured while updating current movie status, please try again."
            };
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            Task<CreateMovieResultModel> responseTaskUpdateMovie = Task.FromResult(createMovieResultModel);
            _mockMoviesService.Setup(x => x.UpdateMovieStatus(It.IsAny<Guid>())).Returns(responseTaskUpdateMovie);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.UpdateMovieCurrentStatus(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(createMovieResultModel.ErrorMessage, resultErrorResponseModel);
            Assert.AreEqual(statusCodeExpected, resultStatusCodeIntoInt);
        }

        public void MoviesController_GetCurrent_ReturnOkObjectResult()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            IEnumerable<MovieDomainModel> movieDomainModels = _listOfMovieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrent().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var resultList = ((List<MovieDomainModel>)result);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedCount, resultList.Count);
            Assert.AreEqual(_movieDomainModel.Id, resultList[0].Id);
        }

        [TestMethod]
        public void MoviesController_GetCurrent_ReturnStatusCodeObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie does not exist.";

            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMovies()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrent().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
        }

        [TestMethod]
        public void MoviesController_GetCurrent_ReturnBadRequestObjectResult()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesService.Setup(x => x.GetCurrentMovies()).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_mockILogger.Object, _mockMoviesService.Object, _mockProjectionsService.Object, _mockTagService.Object, _mockSeatService.Object);
            //Act
            var resultAction = moviesController.GetCurrent().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


    }
}
