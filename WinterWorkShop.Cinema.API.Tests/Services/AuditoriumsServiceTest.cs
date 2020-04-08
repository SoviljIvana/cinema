using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;


namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class AuditoriumsServiceTest
    {
        private Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Mock<ISeatsRepository> _mockSeatsRepository;
        private Mock<IProjectionsRepository> _mockProjectionRepository;
        private Mock<ITicketService> _mockTicketService;
        private Auditorium _auditorium;
        private Auditorium _newAuditorium;
        private Data.Cinema _cinema;
        private Projection _projection;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private List<Auditorium> _listOFAuditoriums;
        private List<Projection> _listOfProjections;


        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema()
            {
                Name = "CinemaName",
                Id = 1
            };
            _auditorium = new Auditorium()
            {
                Id = 1,
                CinemaId = 1,
                Name = "AuditoriumName",
                Seats = new List<Seat>(),
                Cinema = _cinema
            };
            _auditoriumDomainModel = new AuditoriumDomainModel()
            {
                Id = 1,
                Name = "AuditoriumName",
                CinemaId = 1,
                CinemaName = "CinemaName"
            };
            _newAuditorium = new Auditorium();
            _newAuditorium = _auditorium;
            _newAuditorium.Seats.Add(new Seat()
            {
                Number = 1,
                Row = 1,
                AuditoriumId = 1
            });
            _projection = new Projection()
            {
                AuditoriumId = 1,
                DateTime = DateTime.Now.AddDays(1),
                Id = Guid.NewGuid(),
                MovieId = Guid.NewGuid()
            };
            _listOfProjections = new List<Projection>();
            _listOfProjections.Add(_projection);
            _listOFAuditoriums = new List<Auditorium>();
            _listOFAuditoriums.Add(_auditorium);

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockSeatsRepository = new Mock<ISeatsRepository>();
            _mockProjectionRepository = new Mock<IProjectionsRepository>();
            _mockTicketService = new Mock<ITicketService>();
        }

        [TestMethod]
        public void AuditoriumsService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)resultAction;
            //Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void AuditoriumsService_GetAllAsync_ReturnListOfAuditoriumDomainModel()
        {
            //Arrange
            int expectedResultCount = 1;
            IEnumerable<Auditorium> auditoriums = _listOFAuditoriums;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_auditorium.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
        }

        [TestMethod]
        public void AuditoriumsService_GetAllOfSpecificCinema_ReturnNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = null;
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(auditoriums);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.GetAllOfSpecificCinema(It.IsAny<int>());
            var result = (List<AuditoriumDomainModel>)resultAction;
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumsService_GetAllOfSpecificCinema_ReturnListOfAuditoriumDomainModel()
        {
            //Arrange
            int expectedResultCount = 1;
            IEnumerable<Auditorium> auditoriums = _listOFAuditoriums;
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(auditoriums);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.GetAllOfSpecificCinema(It.IsAny<int>());
            var result = (List<AuditoriumDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_auditorium.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
        }
        [TestMethod]
        public void AuditoriumsService_CreateAuditorium_ReturnCreateAuditoriumResultModel_InvalidCinemaId()
        {
            //Arrange
            CreateAuditoriumResultModel expectedResultModel = new CreateAuditoriumResultModel()
            {
                ErrorMessage = "Cannot create new auditorium, auditorium with given cinemaId does not exist.",
                IsSuccessful = false
            };
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            var numberOfSeatsPerRow = 2;
            var numberOfRows = 2;
            Data.Cinema cinema = null;
            Task<Data.Cinema> responseTask = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, numberOfRows, numberOfSeatsPerRow).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(CreateAuditoriumResultModel));
        }
        [TestMethod]
        public void AuditoriumsService_CreateAuditorium_ReturnCreateAuditoriumResultModel_AuditoriumWithSameNameExists()
        {
            //Arrange
            CreateAuditoriumResultModel expectedResultModel = new CreateAuditoriumResultModel()
            {
                ErrorMessage = "Cannot create new auditorium, auditorium with same name already exist.",
                IsSuccessful = false
            };
            IEnumerable<Auditorium> auditoriums = _listOFAuditoriums;
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            var numberOfSeatsPerRow = 2;
            var numberOfRows = 2;
            Data.Cinema cinema = _cinema ;
            Task<Data.Cinema> responseTaskCinemasRepository = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskCinemasRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            Task<IEnumerable<Auditorium>> responseTaskAuditoriumsRepository = Task.FromResult(auditoriums);
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(auditoriumDomainModel.Name, cinema.Id)).Returns(responseTaskAuditoriumsRepository);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, numberOfRows, numberOfSeatsPerRow).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(CreateAuditoriumResultModel));
        }
        [TestMethod]
        public void AuditoriumsService_CreateAuditorium_ReturnCreateAuditoriumResultModel_AUDITORIUM_CREATION_ERROR()
        {
            //Arrange
            CreateAuditoriumResultModel expectedResultModel = new CreateAuditoriumResultModel()
            {
                ErrorMessage = "Error occured while creating new auditorium, please try again.",
                IsSuccessful = false
            };
            IEnumerable<Auditorium> auditoriums = _listOFAuditoriums;
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Auditorium auditoriumNull = null;
            Auditorium auditorium = _auditorium;
            var numberOfSeatsPerRow = 2;
            var numberOfRows = 2;
            Data.Cinema cinema = _cinema;
            Task<Data.Cinema> responseTaskCinemasRepository = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskCinemasRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            Task<IEnumerable<Auditorium>> responseTaskAuditoriumsRepository = Task.FromResult(auditoriums);
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(auditoriumDomainModel.Name, cinema.Id)).Returns(responseTaskAuditoriumsRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.Insert(auditorium)).Returns(auditoriumNull);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, numberOfRows, numberOfSeatsPerRow).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(CreateAuditoriumResultModel));
        }
        [TestMethod]
        public void AuditoriumsService_CreateAuditorium_ReturnCreateAuditoriumResultModel_Successful()
        {
            IEnumerable<Auditorium> auditoriums = _listOFAuditoriums;
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Auditorium auditorium = _auditorium;
            Auditorium newAuditorium = _newAuditorium;
            var numberOfSeatsPerRow = 1;
            var numberOfRows = 1;
            Data.Cinema cinema = _cinema;
            Task<Data.Cinema> responseTaskCinemasRepository = Task.FromResult(cinema);
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskCinemasRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            Task<IEnumerable<Auditorium>> responseTaskAuditoriumsRepository = Task.FromResult(auditoriums);
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(auditoriumDomainModel.Name, cinema.Id)).Returns(responseTaskAuditoriumsRepository);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(newAuditorium);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, numberOfRows, numberOfSeatsPerRow).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(CreateAuditoriumResultModel));
            Assert.AreEqual(1, resultAction.Auditorium.Id);
        }

        [TestMethod]
        public void AuditoriumsService_GetAuditoriumByIdAsync_ReturnNull()
        {
            //Arrange
            Auditorium auditorium = null;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.GetAuditoriumByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void AuditoriumsService_GetAuditoriumByIdAsync_ReturnAuditoriumDomainModel()
        {
            //Arrange
            Auditorium auditorium = _auditorium;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.GetAuditoriumByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumDomainModel));
            Assert.AreEqual(resultAction.CinemaId, auditorium.CinemaId);
        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_Returns_AuditoriumResultModel_AuditoriumUpdateError_ProjectionInFuture()
        {
            //Arrange
            AuditoriumResultModel expectedResultModel = new AuditoriumResultModel()
            {
                ErrorMessage = "Unable to update auditorium, please make sure there no upcoming projections and then try again. "
            };
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            IEnumerable<Projection> projections = _listOfProjections;
            _mockProjectionRepository = new Mock<IProjectionsRepository>();
            _mockProjectionRepository.Setup(x=>x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(_listOfProjections);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.UpdateAuditorium(auditoriumDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumResultModel));
        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_Returns_AuditoriumResultModel_AuditoriumUpdateError()
        {
            //Arrange
            AuditoriumResultModel expectedResultModel = new AuditoriumResultModel()
            {
                ErrorMessage = "Error happened when auditorium was updating. "
            };
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Auditorium auditorium = null;
            IEnumerable<Projection> projections = null;
            IEnumerable<Seat> seats = null;
            _mockProjectionRepository = new Mock<IProjectionsRepository>();
            _mockProjectionRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(projections);
            _mockSeatsRepository = new Mock<ISeatsRepository>();
            _mockSeatsRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(seats);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(auditorium);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.UpdateAuditorium(auditoriumDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumResultModel));
        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_Returns_AuditoriumResultModel_Successful()
        {
            //Arrange
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Auditorium auditorium = _auditorium;
            IEnumerable<Projection> projections = null;
            IEnumerable<Seat> seats = null;
            _mockProjectionRepository = new Mock<IProjectionsRepository>();
            _mockProjectionRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(projections);
            _mockSeatsRepository = new Mock<ISeatsRepository>();
            _mockSeatsRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(seats);
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(auditorium);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.UpdateAuditorium(auditoriumDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumResultModel));
            Assert.AreEqual(auditoriumDomainModel.Name, resultAction.Auditorium.Name);
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditorium_AuditoriumResultModel_AUDITORIUM_DOES_NOT_EXIST()
        {
            //Arrange
            AuditoriumResultModel expectedResultModel = new AuditoriumResultModel()
            {
                ErrorMessage = "Auditorium does not exist."
            };
            Auditorium auditorium = null;
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);

            //Act
            var resultAction = auditoriumController.DeleteAuditorium(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumResultModel));
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditorium_AuditoriumResultModel_PROJECTION_IN_FUTURE()
        {
            //Arrange
            AuditoriumResultModel expectedResultModel = new AuditoriumResultModel()
            {
                ErrorMessage = "Cannot delete projection as it is scheduled in the future. "
            };
            Auditorium auditorium = _auditorium;
            IEnumerable<Projection> projections = _listOfProjections;
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            _mockProjectionRepository = new Mock<IProjectionsRepository>();
            _mockProjectionRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(projections);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);

            //Act
            var resultAction = auditoriumController.DeleteAuditorium(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedResultModel.ErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumResultModel));
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditorium_AuditoriumResultModel_Successful()
        {
            //Arrange
            Auditorium auditorium = _auditorium;
            IEnumerable<Projection> projections = null;
            IEnumerable<Seat> seats = null;
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            Task<Auditorium> responseTaskAuditoriumsRepository = Task.FromResult(auditorium);
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTaskAuditoriumsRepository);
            _mockProjectionRepository = new Mock<IProjectionsRepository>();
            _mockProjectionRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(projections);
            _mockSeatsRepository = new Mock<ISeatsRepository>();
            _mockSeatsRepository.Setup(x => x.GetAllOfSpecificAuditorium(It.IsAny<int>())).Returns(seats);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatsRepository.Object, _mockProjectionRepository.Object, _mockTicketService.Object);
            //Act
            var resultAction = auditoriumController.DeleteAuditorium(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(AuditoriumResultModel));
            Assert.AreEqual(auditorium.Name, resultAction.Auditorium.Name);
        }
    }
}
