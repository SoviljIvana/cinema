using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Data.Cinema _cinema;
        private CinemaDomainModel _cinemaDomainModel;
        private Data.Cinema _newCinema;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema
            {
                Id = 1,
                Name = "CinemaName",
                Auditoriums = new List<Auditorium>()                              
            };

            _cinema.Auditoriums.Add(new Auditorium
            {
                Name = "AuditoriumName",
                Id = 1,
                CinemaId = 1,
                Cinema = new Data.Cinema
                {
                    Name = "CinemaName",
                }
            });

            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "CinemaName2"
            };

            _newCinema = new Data.Cinema { 
                Id = 1,
                Name = "NewCinemaName",
                Auditoriums = new List<Auditorium>()
                
            };
            

            List<Data.Cinema> cinemasModelsList = new List<Data.Cinema>();
            
            cinemasModelsList.Add(_cinema);

            IEnumerable<Data.Cinema> cinemas = cinemasModelsList;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);

            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_newCinema);

        }

        //[TestMethod]
        //public void CinemaService_GetAllAsync_ReturnNull()
        //{
        //    //Arrange
        //    IEnumerable<Data.Cinema> cinemas = null;
        //    Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);
            
        //    _mockCinemasRepository = new Mock<ICinemasRepository>();
        //    _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
        //    CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object);

        //    //Act
        //    var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        //    //Assert
        //    Assert.IsNull(resultAction);
        //}
        //[TestMethod]
        //public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        //{
        //    //Arrange
        //    int expectedResultCount = 1;
        //    CinemaService projectionController = new CinemaService(_mockCinemasRepository.Object);
        //    //Act
        //    var resultAction = projectionController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        //    var result = (List<CinemaDomainModel>)resultAction;
        //    //Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(expectedResultCount, result.Count);
        //    Assert.AreEqual(_cinema.Id, result[0].Id);
        //    Assert.IsInstanceOfType(result[0], typeof(CinemaDomainModel));
        //}

        //[TestMethod]
        //public void CinemaService_CreateCinema_HappyFlow()
        //{
        //    CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object);

        //    var result = cinemaService.AddCinema(_cinemaDomainModel).Result;

        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.IsSuccessful);
        //    Assert.IsNull(result.ErrorMessage);
        //    Assert.IsNotNull(result.Cinema);
        //    Assert.AreEqual(_newCinema.Id, result.Cinema.Id);
        //}

    }
}
    

