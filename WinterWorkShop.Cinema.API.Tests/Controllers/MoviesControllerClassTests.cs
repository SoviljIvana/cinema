using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerClassTests
    {
        private Mock<IMovieService> _movieService;

        //ja sam zakomentarisala jer sam menjala kontroler, //JelenaZubac

        //[TestMethod]
        //public void GetAsync_Return_All_Movies()
        //{
        //    //Arrange
        //    List<MovieDomainModel> movieModelsList = new List<MovieDomainModel>();
        //    MovieDomainModel movie = new MovieDomainModel
        //    {
        //        Current = true,
        //        Id = Guid.NewGuid(),
        //        Rating = 9.5,
        //        Title = "New Title",
        //        Year = 2010
        //    };

        //    movieModelsList.Add(movie);
        //    IEnumerable<MovieDomainModel> movies = movieModelsList;
        //    int expectedStatusCode = 200;

        //    _movieService = new Mock<IMovieService>();
        //    _movieService.Setup(x => x.GetCurrentMovies(true)).Returns(movies);
        //}

    }
}
