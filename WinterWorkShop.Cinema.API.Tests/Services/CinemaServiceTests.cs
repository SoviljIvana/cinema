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

            List<Data.Cinema> cinemasModelsList = new List<Data.Cinema>();
            
            cinemasModelsList.Add(_cinema);
            IEnumerable<Data.Cinema> cinemas = cinemasModelsList;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        {
            //Arrange
            int expectedResultCount = 1;
            CinemaService projectionController = new CinemaService(_mockCinemasRepository.Object);
            //Act
            var resultAction = projectionController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(CinemaDomainModel));
        }

    }
}
    

