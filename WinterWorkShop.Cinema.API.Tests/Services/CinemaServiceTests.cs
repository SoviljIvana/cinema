using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Mock<ITicketService> _mockTicketService;
        private Mock<ISeatsRepository> _mockSeatsRepository;

        private Data.Cinema _cinema;
        private List<Data.Cinema> _listOfCinemas;
        private CinemaDomainModel _newCinema;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema
            {
                Id = 1,
                Name = "CinemaName",
                Auditoriums = new List<Auditorium>()
            };
            _newCinema = new CinemaDomainModel
            {
                Id = 1,
                Name = "CinemaName"
            };
            _listOfCinemas = new List<Data.Cinema>();
            _listOfCinemas.Add(_cinema);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockSeatsRepository = new Mock<ISeatsRepository>();
            _mockTicketService = new Mock<ITicketService>();

        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Data.Cinema> cinemas = null;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object, 
                                                                _mockAuditoriumService.Object, 
                                                                _mockAuditoriumsRepository.Object,
                                                                _mockProjectionsRepository.Object,
                                                                _mockTicketService.Object,
                                                                _mockSeatsRepository.Object);

            //Act
            var resultAction = cinemaController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        {
            //Arrange
            int expectedResultCount = 1;
            IEnumerable<Data.Cinema> cinemas = _listOfCinemas;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                                                _mockAuditoriumService.Object,
                                                                _mockAuditoriumsRepository.Object,
                                                                _mockProjectionsRepository.Object,
                                                                _mockTicketService.Object,
                                                                _mockSeatsRepository.Object);
            //Act
            var resultAction = cinemaController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_AddCinema_HappyFlow()
        {
            //Arrange
            Data.Cinema cinema = _cinema;
            CinemaDomainModel newCinema = _newCinema;
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(cinema);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                                                _mockAuditoriumService.Object,
                                                                _mockAuditoriumsRepository.Object,
                                                                _mockProjectionsRepository.Object,
                                                                _mockTicketService.Object,
                                                                _mockSeatsRepository.Object);
            //Act
            var resultAction = cinemaController.AddCinema(newCinema).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsNotNull(resultAction.Cinema);
            Assert.AreEqual(_newCinema.Id, resultAction.Cinema.Id);
        }
    }
}


