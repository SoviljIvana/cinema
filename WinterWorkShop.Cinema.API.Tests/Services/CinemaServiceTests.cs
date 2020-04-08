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
        private Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Mock<ITicketService> _mockTicketService;
        private Mock<ISeatsRepository> _mockSeatsRepository;

        private Data.Cinema _cinema;
        private List<Data.Cinema> _listOfCinemas;
        private CinemaDomainModel _newCinema;
        private CreateCinemaDomainModel _createCinemaDomainModel;
        private CinemaDomainModel _cinemaDomainModel;
        private Auditorium _auditorium;
        private List<Auditorium> _listOfAuditorium;
        private Projection _projection;
        private Projection _projection2;
        private List<Projection> _listOfProjections;
        private List<Projection> _listOfProjectionsNoInFuture;
        private Seat _seat;
        private List<Seat> _listofSeats;

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
            _createCinemaDomainModel = new CreateCinemaDomainModel()
            {
                CinemaName = "CinemaName",
                listOfAuditoriums = new List<AuditoriumDomainModel>()
            };
            _cinemaDomainModel = new CinemaDomainModel()
            {
                Id = 1,
                Name = "CinemaName"
            };
            _auditorium = new Auditorium()
            {
                Name = "AuditoriumName",
                Id = 1,
                Projections = new List<Projection>(),
                Seats = new List<Seat>()
            };
            _projection = new Projection()
            {
                DateTime = DateTime.Now.AddDays(1),
                Id = Guid.NewGuid()
            };
            _projection2 = new Projection()
            {
                DateTime = DateTime.Now,
                Id = Guid.NewGuid()
            };
            _seat = new Seat()
            {
                Id = Guid.NewGuid()
            };
            _listOfCinemas = new List<Data.Cinema>();
            _listOfCinemas.Add(_cinema);
            _listOfAuditorium = new List<Auditorium>();
            _listOfAuditorium.Add(_auditorium);
            _listOfProjections = new List<Projection>();
            _listOfProjections.Add(_projection);
            _listOfProjectionsNoInFuture = new List<Projection>();
            _listOfProjectionsNoInFuture.Add(_projection2);
            _listofSeats = new List<Seat>();
            _listofSeats.Add(_seat);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
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
        public void CinemasService_AddCinemaWithAuditoriumsAndSeats_Return_ErrorMessage_CinemaSameName()
        {
            //Arrange
            var expectedErrorMessage = "Cannot create new cinema, cinema with same name alredy exist.";
            CreateCinemaDomainModel createCinemaDomainModel = _createCinemaDomainModel;
            Data.Cinema cinema = _cinema;
            Task<Data.Cinema> responseTask = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByNameAsync(It.IsAny<string>())).Returns(responseTask);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                                    _mockAuditoriumsRepository.Object,
                                                    _mockProjectionsRepository.Object,
                                                    _mockTicketService.Object,
                                                    _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.AddCinemaWithAuditoriumsAndSeats(createCinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Cinema);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, result.ErrorMessage);
        }
        [TestMethod]
        public void CinemasService_AddCinemaWithAuditoriumsAndSeats_Return_ErrorMessage_CinemaCreationMessage()
        {
            //Arrange
            var expectedErrorMessage = "Error occured while creating new cinema, please try again.";
            CreateCinemaDomainModel createCinemaDomainModel = _createCinemaDomainModel;
            Data.Cinema cinema = null;
            Data.Cinema insertedCinema = _cinema;
            Task<Data.Cinema> responseTask = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByNameAsync(It.IsAny<string>())).Returns(responseTask);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(insertedCinema);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                                    _mockAuditoriumsRepository.Object,
                                                    _mockProjectionsRepository.Object,
                                                    _mockTicketService.Object,
                                                    _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.AddCinemaWithAuditoriumsAndSeats(createCinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Cinema);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(insertedCinema.Name, result.Cinema.Name);
        }
        [TestMethod]
        public void CinemasService_GetCinemaByIdAsync_Return_null()
        {
            //Arrange
            Data.Cinema cinema = null;
            Task<Data.Cinema> responseTask = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.GetCinemaByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void CinemasService_GetCinemaByIdAsync_Return_CinemaDomainModel()
        {
            //Arrange
            Data.Cinema cinema = _cinema;
            Task<Data.Cinema> responseTask = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.GetCinemaByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(cinema.Id, result.Id);
        }
        [TestMethod]
        public void CinemasService_UpdateCinema_Return_null()
        {
            //Arrange
            CinemaDomainModel cinemaDomainModel = _cinemaDomainModel;
            Data.Cinema cinema = null;
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(cinema);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.UpdateCinema(cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void CinemasService_UpdateCinema_Return_CinemaDomainModel()
        {
            //Arrange
            CinemaDomainModel cinemaDomainModel = _cinemaDomainModel;
            Data.Cinema cinema = _cinema;
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(cinema);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.UpdateCinema(cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
            Assert.AreEqual(cinema.Name, result.Name);
        }
        [TestMethod]
        public void CinemasService_DeleteCinema_Return_Messages_CinemaDoesNotExsit()
        {
            //Arrange
            var expectedErrorMessage = "Cinema does not exist.";
            Data.Cinema cinema = null;
            Task<Data.Cinema> responseTaskOfCinemaRepository = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfCinemaRepository);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, result.ErrorMessage);
        }
        [TestMethod]
        public void CinemasService_DeleteCinema_Return_CreateCinemaResultModel_WhenThereAreNotAditoriumsInCinema()
        {
            //Arrange
            CinemaResultModel cinemaResultModel = new CinemaResultModel() 
            {
                Cinema = _cinemaDomainModel
            };
            Data.Cinema cinema = _cinema;
            IEnumerable<Auditorium> auditorium = null;
            Task<Data.Cinema> responseTaskOfCinemaRepository = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfCinemaRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(auditorium);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Cinema);
            Assert.IsInstanceOfType(result.Cinema, typeof(CinemaDomainModel));
            Assert.IsInstanceOfType(result, typeof(CinemaResultModel));
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_cinemaDomainModel.Name, result.Cinema.Name);
        }
        [TestMethod]
        public void CinemasService_DeleteCinema_Return_ErrorMessage_AuditoriumDoesNotExsit()
        {
            //Arrange
            var expectedErrorMessage = "Auditorium does not exist.";

            Data.Cinema cinema = _cinema;
            Auditorium auditoriums = null;
            IEnumerable<Auditorium> listOfAuditoriums = _listOfAuditorium;
            Task<Data.Cinema> responseTaskOfCinemaRepository = Task.FromResult(cinema);
            Task<Auditorium> responseTaskOfAuditoriumRepository = Task.FromResult(auditoriums);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfCinemaRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(listOfAuditoriums);
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfAuditoriumRepository);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedErrorMessage, result.ErrorMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CinemaResultModel));
        }
        [TestMethod]
        public void CinemasService_DeleteCinema_Return_ErrorMessage_ProjectionInFuture()
        {
            //Arrange
            var expectedErrorMessage = "Cannot delete projection as it is scheduled in the future. ";

            Data.Cinema cinema = _cinema;
            Auditorium auditorium = _auditorium;
            IEnumerable<Auditorium> listOfAuditoriums = _listOfAuditorium;
            IEnumerable<Projection> listOfProjections = _listOfProjections;
            Task<Data.Cinema> responseTaskOfCinemaRepository = Task.FromResult(cinema);
            Task<Auditorium> responseTaskOfAuditoriumRepository = Task.FromResult(auditorium);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfCinemaRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(listOfAuditoriums);
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfAuditoriumRepository);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAllOfSpecificAuditorium(auditorium.Id)).Returns(listOfProjections);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedErrorMessage, result.ErrorMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CinemaResultModel));
        }
        [TestMethod]
        public void CinemasService_DeleteCinema_Return_CinemaResultModel()
        {
            //Arrange

            Data.Cinema cinema = _cinema;
            Auditorium auditorium = _auditorium;
            IEnumerable<Auditorium> listOfAuditoriums = _listOfAuditorium;
            IEnumerable<Projection> listOfProjections = null;
            Task<Data.Cinema> responseTaskOfCinemaRepository = Task.FromResult(cinema);
            Task<Auditorium> responseTaskOfAuditoriumRepository = Task.FromResult(auditorium);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfCinemaRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(listOfAuditoriums);
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskOfAuditoriumRepository);
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAllOfSpecificAuditorium(auditorium.Id)).Returns(listOfProjections);
            CinemaService cinemaController = new CinemaService(_mockCinemasRepository.Object,
                                        _mockAuditoriumsRepository.Object,
                                        _mockProjectionsRepository.Object,
                                        _mockTicketService.Object,
                                        _mockSeatsRepository.Object);
            //Act
            var result = cinemaController.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CinemaResultModel));
        }
    }
}


