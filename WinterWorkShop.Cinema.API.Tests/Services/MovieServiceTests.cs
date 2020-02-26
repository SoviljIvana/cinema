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
        public List<Movie> moviesModelsList;

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
            _movie.MovieTags.Add(new MovieTag
            {
                Id = Guid.NewGuid(),
                MovieId = _movie.Id,
                TagId = 1
            });
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
                Id = Guid.NewGuid(),
                Rating = 10,
                Title = "MovieTitleDomain",
                Year = 2000
            };

            moviesModelsList = new List<Movie>();
            moviesModelsList.Add(_movie);

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
            //Assert.AreEqual(_movie.Current, true);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

    }
}
