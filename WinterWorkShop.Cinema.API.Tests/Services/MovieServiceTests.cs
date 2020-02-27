using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class MovieServiceTests
    {
        private Mock<IMoviesRepository> _mockMoviesRepository;
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Mock<IMovieTagsRepository> _mockMovieTagsRepository;
        private Mock<ITicketRepository> _mockTicketRepository;
        private Movie _movie;
        private MovieDomainModel _movieDomainModel;
        private Movie _newMovie;
        private DeleteMovieModel _deletedMovieModelSuccess;
        private Projection _projection;
        private List<Projection> _projections;
        private List<Movie> moviesModelsList;
        private List<MovieTag> _movieTagsList;
        private List<MovieDomainModel> _movieDomainModels;
        private MovieTag _movieTag;


        [TestInitialize]
        public void TestInitialize()
        {
            _movie = new Data.Movie
            {
                Id = Guid.NewGuid(),
                Current = true,
                Rating = 2,
                Title = "TitleMovie",
                MovieTags = new List<MovieTag>(),
                Projections = new List<Projection>(),
                Year = 1895
            };
            _movieTag = new MovieTag
            {
                Id = Guid.NewGuid(),
                MovieId = _movie.Id,
                TagId = 1,
                Tag = new Tag {
                    Id  = 1,
                    Name = "NameTag",
                    Type = "Type"
                }

            };
            _movie.MovieTags.Add(_movieTag);
            _movie.Projections.Add(new Projection
            {
                AuditoriumId = 1,
                DateTime = DateTime.Now,
                Id = Guid.NewGuid(),
                MovieId = _movie.Id
            });
            _movieDomainModel = new MovieDomainModel
            {
                Current = true,
                Id = _movie.Id,
                Rating = 2,
                Title = "TitleMovie",
                Year = 1895
            };
            _deletedMovieModelSuccess = new DeleteMovieModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                MovieDomainModel = _movieDomainModel
            
            };
            _projection = new Projection 
            {
                Id = Guid.NewGuid(),
                MovieId = _movie.Id,
                AuditoriumId = 1,
                DateTime = DateTime.Now.AddDays(1)
            };
            

            _projections = new List<Projection>();
            _projections.Add(_projection);
            moviesModelsList = new List<Movie>();
            moviesModelsList.Add(_movie);
            _movieTagsList = new List<MovieTag>();
            _movieTagsList.Add(_movieTag);
            _movieDomainModels = new List<MovieDomainModel>();
            _movieDomainModels.Add(_movieDomainModel);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockMovieTagsRepository = new Mock<IMovieTagsRepository>();
            _mockTicketRepository = new Mock<ITicketRepository>();
        }

        [TestMethod]
        public void MovieService_GetCurrentAndNotCurrentMovies_ReturnNull()
        {
            //Arrange
            IEnumerable<Movie> movies = null;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetCurrentAndNotCurrentMovies()).Returns(responseTask);
            
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetCurrentAndNotCurrentMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void MovieService_GetCurrentAndNotCurrentMovies_ReturnListOfMovies() 
        {
            //Arrange
            int expectedResultCount = 1;
            IEnumerable<Movie> movies = moviesModelsList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetCurrentAndNotCurrentMovies()).Returns(responseTask);
            
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetCurrentAndNotCurrentMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }
        [TestMethod]
        public void MovieService_GetCurrentMovies_ReturnNull()
        {
            IEnumerable<Movie> movies = null;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetCurrent()).Returns(responseTask);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetCurrentMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void MovieService_GetCurrentMovies_ReturnListOfMovies()
        {
            //Arrange
            int expectedResultCount = 1;
            IEnumerable<Movie> movies = moviesModelsList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetCurrent()).Returns(responseTask);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetCurrentMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.AreEqual(_movie.Current, true);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

        //GetTopTenMovies
        [TestMethod]
        public void MovieService_GetTopTenMovies_ReturnNull()
        {
            IEnumerable<Movie> movies = null;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetTopTenMovies()).Returns(responseTask);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetTopTenMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void MovieService_GetTopTenMovies_ReturnListOfMovies()
        {
            //Arrange
            int expectedResultCount = 1;
            
            IEnumerable<Movie> movies = moviesModelsList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetTopTenMovies()).Returns(responseTask);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetTopTenMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_DeleteMovie_ReturnDeletedMovieModel_Successful()
        {
            //Arrange
            var expectedResult = _deletedMovieModelSuccess;
            Movie movie = _movie;
            var id = Guid.NewGuid();
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.Delete(id)).Returns(movie);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act

            var resultAction = movieController.DeleteMovie(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.MovieDomainModel.Id, result.MovieDomainModel.Id);
            Assert.IsInstanceOfType(result, typeof(DeleteMovieModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
        }

        [TestMethod]
        public void MovieService_DeleteMovie_ReturnDeletedMovieModel_ProjectionInFuture()
        {
            //Arrange
            var expectedResult = new DeleteMovieModel();
            Movie movie = _movie;
            movie.Projections.Add(new Projection
            {
                Id = Guid.NewGuid(),
                MovieId = _movie.Id,
                AuditoriumId = 1,
                DateTime = DateTime.Now.AddDays(1)
            });
            var id = Guid.NewGuid();
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.Delete(id)).Returns(movie);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.DeleteMovie(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DeleteMovieModel));
        }

        [TestMethod]
        public void MovieService_DeleteMovie_ReturnNull()
        {
            //Arrange
            Movie movie = null;
            var id = Guid.NewGuid();
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.Delete(id)).Returns(movie);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = movieController.DeleteMovie(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void MovieService_GetMovieByIdAsync_ReturnNull()
        {
            //Arrange
            Movie movie = null;
            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetMovieByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void MovieService_GetMovieByIdAsync_ReturnMovieDomainModel()
        {
            //Arrange
            Movie movie = _movie;
            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.GetMovieByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_movie.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(MovieDomainModel));
        }

        //UpdateMovieStatus
        [TestMethod]
        public void MovieService_UpdateMovieStatus_ReturnNull()
        {
            //Arrange
            Movie movie = null;
            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovieStatus(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void MovieService_UpdateMovieStatus_ReturnCreateMovieResultModel_NotSuccessful_ProjectionInFuture()
        {
            //Arrange
            Movie movie = _movie;
            var expectedResult = new CreateMovieResultModel()
            {
                ErrorMessage = "Error occured while updating current movie status. This movie has projection in future, so it can not be not current.",
                IsSuccessful = false
            };
            IEnumerable<Projection> listaProjekcija = _projections;

            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);

            Task<IEnumerable<Projection>> responseTaskProjection = Task.FromResult(listaProjekcija);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(y => y.GetAllFromOneMovie(id)).Returns(responseTaskProjection);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovieStatus(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreateMovieResultModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
        }

        //domainModel
        [TestMethod]
        public void MovieService_UpdateMovieStatus_ReturnCreateMovieResultModel_NotSuccessful_Update()
        {
            Movie movie = _movie;
            Movie movieNull = null;
            var expectedResult = new CreateMovieResultModel()
            {
                ErrorMessage = "Error occured while updating current movie status, please try again.",
                IsSuccessful = false
            };
            IEnumerable<Projection> listaProjekcija = new List<Projection>();

            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);

            Task<IEnumerable<Projection>> responseTaskProjection = Task.FromResult(listaProjekcija);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(y => y.GetAllFromOneMovie(id)).Returns(responseTaskProjection);
            _mockMoviesRepository.Setup(x => x.Update(movie)).Returns(movieNull);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovieStatus(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreateMovieResultModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
        }

        [TestMethod]
        public void MovieService_UpdateMovieStatus_ReturnCreateMovieResultModel_Successful_Update()
        {
            Movie movie = _movie;
            Movie movieUpdated = _movie;
            movieUpdated.Current = false;
            var expectedResult = new CreateMovieResultModel()
            {
                ErrorMessage = null,
                IsSuccessful = true
            };
            IEnumerable<Projection> listaProjekcija = new List<Projection>();

            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);

            Task<IEnumerable<Projection>> responseTaskProjection = Task.FromResult(listaProjekcija);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(y => y.GetAllFromOneMovie(id)).Returns(responseTaskProjection);

            _mockMoviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(movieUpdated);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovieStatus(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreateMovieResultModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
            Assert.IsFalse(result.Movie.Current);
        }


        [TestMethod]
        public void MovieService_UpdateMovie_ReturnCreateMovieResultModel_NotSuccessful_ProjectionInFuture()
        {
            //Arrange
            Movie movie = _movie;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            movieDomainModel.Current = false;

            var expectedResult = new CreateMovieResultModel()
            {
                ErrorMessage = "Error occured while updating current movie status. This movie has projection in future, so it can not be not current.",
                IsSuccessful = false
            };
            IEnumerable<Projection> listaProjekcija = _projections;

            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(_movieDomainModel.Id)).Returns(responseTask);

            Task<IEnumerable<Projection>> responseTaskProjection = Task.FromResult(listaProjekcija);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(y => y.GetAllFromOneMovie(_movieDomainModel.Id)).Returns(responseTaskProjection);
            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovie(movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreateMovieResultModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
        }

        [TestMethod]
        public void MovieService_UpdateMovie_ReturnCreateMovieResultModel_NotSuccessful_Update()
        {
            Movie movie = _movie;
            MovieDomainModel movieDomainModel = _movieDomainModel;
            Movie movieNull = null;
            var expectedResult = new CreateMovieResultModel()
            {
                ErrorMessage = "Error occured while updating current movie status, please try again.",
                IsSuccessful = false
            };
            IEnumerable<Projection> listaProjekcija = new List<Projection>();

            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(_movieDomainModel.Id)).Returns(responseTask);

            Task<IEnumerable<Projection>> responseTaskProjection = Task.FromResult(listaProjekcija);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(y => y.GetAllFromOneMovie(_movieDomainModel.Id)).Returns(responseTaskProjection);
            _mockMoviesRepository.Setup(x => x.Update(movie)).Returns(movieNull);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovie(movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreateMovieResultModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
        }

        [TestMethod]
        public void MovieService_UpdateMovie_ReturnCreateMovieResultModel_Successful_Update()
        {
            Movie movie = _movie;
            Movie movieUpdated = _movie;
            movieUpdated.Current = false;
            MovieDomainModel movieDomainModel = _movieDomainModel;

            var expectedResult = new CreateMovieResultModel()
            {
                ErrorMessage = null,
                IsSuccessful = true
            };
            IEnumerable<Projection> listaProjekcija = new List<Projection>();

            var id = Guid.NewGuid();
            Task<Movie> responseTask = Task.FromResult(movie);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(_movieDomainModel.Id)).Returns(responseTask);


            Task<IEnumerable<Projection>> responseTaskProjection = Task.FromResult(listaProjekcija);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(y => y.GetAllFromOneMovie(_movieDomainModel.Id)).Returns(responseTaskProjection);

            _mockMoviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(movieUpdated);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreateMovieResultModel));
            Assert.AreEqual(expectedResult.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expectedResult.IsSuccessful, result.IsSuccessful);
            Assert.IsFalse(result.Movie.Current);
        }

        //addMovie
        [TestMethod]
        public void MovieService_AddMovie_ReturnNull()
        {
            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            Movie movie = null; 
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(movie);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.AddMovie(movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void MovieService_AddMovie_ReturnMovieDomainModel()
        {
            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            Movie movie = _movie;
            //MovieDomainModel repositoryResponse = _mov;
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(movie);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);
            //Act
            var resultAction = movieController.AddMovie(movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsInstanceOfType(resultAction, typeof(MovieDomainModel));
            Assert.AreEqual(resultAction.Rating, movieDomainModel.Rating);
            Assert.AreEqual(resultAction.Id, movieDomainModel.Id);
        }

        //GetAllMoviesWithThisTag
        [TestMethod]
        public void MovieService_GetAllMoviesWithThisTag_CreateMovieResultModel_ReturnNull_NotFilmsWithAnyOfThisTags()
        {
            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            string stringToSearch = "string nostring";
            IEnumerable<MovieDomainModel> expectedResult = null;
            
            IEnumerable<MovieTag> movieTags = _movieTagsList;
            Task<IEnumerable<MovieTag>> responseTaskMovieTagRepository = Task.FromResult(movieTags);
            IEnumerable<Movie> movies = moviesModelsList;
            Task<IEnumerable<Movie>> responseTaskMovieRepository = Task.FromResult(movies);

            _mockMovieTagsRepository = new Mock<IMovieTagsRepository>();
            _mockMovieTagsRepository.Setup(x => x.GetAll()).Returns(responseTaskMovieTagRepository);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetAllWithMovieTags()).Returns(responseTaskMovieRepository);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = movieController.GetAllMoviesWithThisTag(stringToSearch).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;

            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void MovieService_GetAllMoviesWithThisTag_CreateMovieResultModel_ReturnNull_NotFilmsWithAllTagsRequired()
        {

            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            string stringToSearch = "string NameTag";
            IEnumerable<MovieDomainModel> expectedResult = null;

            IEnumerable<MovieTag> movieTags = _movieTagsList;
            Task<IEnumerable<MovieTag>> responseTaskMovieTagRepository = Task.FromResult(movieTags);
            IEnumerable<Movie> movies = moviesModelsList;
            Task<IEnumerable<Movie>> responseTaskMovieRepository = Task.FromResult(movies);

            _mockMovieTagsRepository = new Mock<IMovieTagsRepository>();
            _mockMovieTagsRepository.Setup(x => x.GetAll()).Returns(responseTaskMovieTagRepository);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetAllWithMovieTags()).Returns(responseTaskMovieRepository);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = movieController.GetAllMoviesWithThisTag(stringToSearch).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;

            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void MovieService_GetAllMoviesWithThisTag_CreateMovieResultModel_ReturnListWithMovieDomainModels()
        {

            //Arrange
            MovieDomainModel movieDomainModel = _movieDomainModel;
            string stringToSearch = "Type NameTag";
            int expectedResultCount = 1;
            IEnumerable<MovieDomainModel> expectedResult;
            expectedResult = _movieDomainModels;

            IEnumerable<MovieTag> movieTags = _movieTagsList;
            Task<IEnumerable<MovieTag>> responseTaskMovieTagRepository = Task.FromResult(movieTags);
            IEnumerable<Movie> movies = moviesModelsList;
            Task<IEnumerable<Movie>> responseTaskMovieRepository = Task.FromResult(movies);

            _mockMovieTagsRepository = new Mock<IMovieTagsRepository>();
            _mockMovieTagsRepository.Setup(x => x.GetAll()).Returns(responseTaskMovieTagRepository);
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockMoviesRepository.Setup(x => x.GetAllWithMovieTags()).Returns(responseTaskMovieRepository);

            MovieService movieController = new MovieService(_mockMoviesRepository.Object, _mockProjectionsRepository.Object, _mockMovieTagsRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = movieController.GetAllMoviesWithThisTag(stringToSearch).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }
    }
}
