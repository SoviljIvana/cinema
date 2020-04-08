using System;
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
    public class SeatsServiceTests
    {
        private Seat _seat;
        private SeatDomainModel _seatDomainModel;
        private RowsDomainModel _rowsDomainModel;
        private List<SeatDomainModel> _listOfSeatDomainModels;
        private List<Seat> _listOfSeats;
        private Mock<ISeatsRepository> _mockSeatRepository;
        private Mock<ITicketRepository> _mockTicketRepository;
        [TestInitialize]
        public void TestInitialize()
        {
            _seat = new Seat()
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 1,
                Row = 1
            };
            _seatDomainModel = new SeatDomainModel()
            {
                Id = _seat.Id
            };

            _listOfSeats = new List<Seat>();
            _listOfSeats.Add(_seat);
            _listOfSeatDomainModels = new List<SeatDomainModel>();
            _listOfSeatDomainModels.Add(_seatDomainModel);
            _rowsDomainModel = new RowsDomainModel()
            {
                SeatsInRow = _listOfSeatDomainModels
            };
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockSeatRepository = new Mock<ISeatsRepository>();
        }

        [TestMethod]
        public void SeatsService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Seat> seats = null;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetAll()).Returns(responseTask);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void SeatsService_GetAllAsync_ReturnListOfSeats()
        {
            //Arrange
            int expectedResultCount = 1;

            IEnumerable<Seat> seats = _listOfSeats;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetAll()).Returns(responseTask);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>) resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_seat.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(SeatDomainModel));

        }
        [TestMethod]
        public void SeatsService_GetAllSeatsForProjection_ReturnNull()
        {
            //Arrange
            IEnumerable<Seat> seats = null;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetAllOfSpecificProjection(It.IsAny<Guid>())).Returns(responseTask);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.GetAllSeatsForProjection(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void SeatsService_GetAllSeatsForProjection_Return_listOfRows()
        {
            //Arrange
            IEnumerable<Seat> seats = _listOfSeats;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);
            IEnumerable<Data.Entities.Ticket> tickets = null;
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetAllOfSpecificProjection(It.IsAny<Guid>())).Returns(responseTask);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.GetAllForSpecificProjection(It.IsAny<Guid>())).Returns(tickets);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.GetAllSeatsForProjection(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<RowsDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsInstanceOfType(result[0], typeof(RowsDomainModel));
        }
        [TestMethod]
        public void SeatsService_DeleteSeat_Return_null_NoSeats()
        {
            //Arrange
            Data.Seat seat = null;
            Task<Seat> responseTask = Task.FromResult(seat);
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.DeleteSeat(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void SeatsService_DeleteSeat_Return_null_NoTicket()
        {
            //Arrange
            Data.Seat seat = _seat;
            seat.Tickets = new List<Data.Entities.Ticket>();
            Task<Seat> responseTask = Task.FromResult(seat);
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.DeleteSeat(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(resultAction);
        }
        [TestMethod]
        public void SeatsService_DeleteSeat_Return_SeatDomainModel()
        {
            //Arrange
            Data.Seat seat = _seat;
            seat.Tickets = null;
            Task<Seat> responseTask = Task.FromResult(seat);
            _mockSeatRepository = new Mock<ISeatsRepository>();
            _mockSeatRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            SeatService seatService = new SeatService(_mockSeatRepository.Object, _mockTicketRepository.Object);

            //Act
            var resultAction = seatService.DeleteSeat(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsInstanceOfType(resultAction, typeof(SeatDomainModel));
            Assert.AreEqual(seat.Id, resultAction.Id);
        }
    }
}
