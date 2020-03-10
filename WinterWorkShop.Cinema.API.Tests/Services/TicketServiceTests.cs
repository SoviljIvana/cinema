using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class TicketServiceTests
    {
        private Mock<ITicketRepository> _mockTicketRepository;
        private Mock<IUsersRepository> _mockUserRepository;
        private Data.User _user;
        private Ticket _ticket;
        private Projection _projection;
        private Movie _movie;
        private Seat _seat;
        private Data.Cinema _cinema;
        private Auditorium _auditorium;
        private TicketDomainModel _ticketDomainModel;
        private List<Ticket> _listOfTickets;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema()
            {
                Name = "name"
            };
            _auditorium = new Auditorium()
            {
                Id = 1,
                Name = "Name",
                Cinema = _cinema
            };
            _seat = new Seat()
            {
                Id = Guid.NewGuid(),
                Auditorium = _auditorium,
                Number = 1,
                Row = 1
            };
            _movie = new Movie()
            {
                Id = Guid.NewGuid(),
                Title = "Title"
            };
            _projection = new Projection()
            {
                Id = Guid.NewGuid(),
                Movie = _movie,
                AuditoriumId = 1,
                DateTime = DateTime.Now,
                MovieId = _movie.Id
            };
            _user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = "UserName",
                BonusPoints = 0,
                FirstName = "FirstName",
                IsAdmin = true,
                LastName = "LastName",
                Tickets = new List<Ticket>()
            };
            _ticketDomainModel = new TicketDomainModel()
            {
                UserName = "UserName",
                Id = Guid.NewGuid(),
                SeatId = Guid.NewGuid()
            };

            _ticket = new Ticket()
            {
                Id = Guid.NewGuid(),
                Paid = false,
                ProjectionId = _projection.Id,
                SeatId = _seat.Id,
                UserId = _user.Id,
                User = _user,
                Projection = _projection,
                Seat = _seat
            };
            _listOfTickets = new List<Ticket>();
            _listOfTickets.Add(_ticket);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockUserRepository = new Mock<IUsersRepository>();

        }

        [TestMethod]
        public void TicketService_CreateNewTicket_Returns_ErrorMessages_UserNotFound()
        {
            //Arrange
            var expectedMessage = "User does not exist.";
            Data.User user = null;
            TicketDomainModel ticketDomainModel = _ticketDomainModel;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var resultAction = ticketService.CreateNewTicket(ticketDomainModel).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            var result = (TicketResultModel) resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(TicketResultModel));
        }
        [TestMethod]
        public void TicketService_CreateNewTicket_Returns_ErrorMessages_ErrorWhileCreatingTicket()
        {
            //Arrange
            var expectedMessage = "Error occured while creating new ticket, please try again.";
            Data.User user = _user;
            TicketDomainModel ticketDomainModel = _ticketDomainModel;
            Ticket ticket = null;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.Insert(It.IsAny<Ticket>())).Returns(ticket);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var resultAction = ticketService.CreateNewTicket(ticketDomainModel).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            var result = (TicketResultModel)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(TicketResultModel));
        }
        [TestMethod]
        public void TicketService_CreateNewTicket_Returns_CreateTicketResultModel()
        {
            //Arrange
            Data.User user = _user;
            TicketDomainModel ticketDomainModel = _ticketDomainModel;
            Ticket ticket = _ticket;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.Insert(It.IsAny<Ticket>())).Returns(ticket);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var resultAction = ticketService.CreateNewTicket(ticketDomainModel).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            var result = (TicketResultModel)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(TicketResultModel));
            Assert.AreEqual(user.Id, result.Ticket.UserId);
        }
        [TestMethod]
        public void TicketService_ConfirmPayment_Returns_PaymentResponse_ErrorMessages_UserNotFound()
        {
            //Arrange
            var expectedMessage = "User does not exist.";
            Data.User user = null;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var result = ticketService.ConfirmPayment(It.IsAny<string>()).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Message);
            Assert.AreEqual(expectedMessage, result.Message);
            Assert.IsInstanceOfType(result, typeof(PaymentResponse));
        }
        [TestMethod]
        public void TicketService_ConfirmPayment_Returns_PaymentResponse_ErrorMessages_TicketsNotFound()
        {
            //Arrange
            var expectedMessage = "Error occured while finding ticket, please try again.";
            Data.User user = _user;
            IEnumerable<Ticket> ticket = null;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(ticket);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.GetAllUnpaidForSpecificUser(It.IsAny<string>())).Returns(responseTask);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var result = ticketService.ConfirmPayment(It.IsAny<string>()).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Message);
            Assert.AreEqual(expectedMessage, result.Message);
            Assert.IsInstanceOfType(result, typeof(PaymentResponse));
        }
        [TestMethod]
        public void TicketService_ConfirmPayment_Returns_PaymentResponse_ErrorMessages_TicketUpdateError()
        {
            //Arrange
            var expectedMessage = "Error occured while updating user, please try again.";
            Data.User user = _user;
            Data.User updatedUser = null;
            IEnumerable<Ticket> listOfTickets = _listOfTickets;
            Ticket ticket = _ticket;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(listOfTickets);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.GetAllUnpaidForSpecificUser(It.IsAny<string>())).Returns(responseTask);
            _mockTicketRepository.Setup(x => x.Update(It.IsAny<Ticket>())).Returns(ticket);
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(updatedUser);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var result = ticketService.ConfirmPayment(It.IsAny<string>()).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Message);
            Assert.AreEqual(expectedMessage, result.Message);
            Assert.IsInstanceOfType(result, typeof(PaymentResponse));
        }
        [TestMethod]
        public void TicketService_ConfirmPayment_Returns_PaymentResponse_Successful()
        {
            //Arrange
            Data.User user = _user;
            Data.User updatedUser = _user;
            IEnumerable<Ticket> listOfTickets = _listOfTickets;
            Ticket ticket = _ticket;
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(listOfTickets);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.GetAllUnpaidForSpecificUser(It.IsAny<string>())).Returns(responseTask);
            _mockTicketRepository.Setup(x => x.Update(It.IsAny<Ticket>())).Returns(ticket);
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(updatedUser);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var result = ticketService.ConfirmPayment(It.IsAny<string>()).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Message);
            Assert.IsInstanceOfType(result, typeof(PaymentResponse));
        }

        [TestMethod]
        public void TicketService_DeleteTicketById_returnNull()
        {
            //Arrange
            Ticket ticket = null;
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(ticket);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);

            //Act
            var result = ticketService.DeleteTicketById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            //Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void TicketService_DeleteTicketById_Returns_TicketResultModel()
        {
            //Arrange
            Ticket ticket = _ticket;
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(ticket);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);

            //Act
            var result = ticketService.DeleteTicketById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(TicketResultModel));
            Assert.AreEqual(ticket.UserId, result.Ticket.UserId);
        }

        [TestMethod]
        public void TicketService_GetAllTicketsForThisUser_Returns_null()
        {
            //Arrange
            IEnumerable<Ticket> listOfTickets = null;
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(listOfTickets);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.GetAllUnpaidForSpecificUser(It.IsAny<string>())).Returns(responseTask);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var result = ticketService.GetAllTicketsForThisUser(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            //Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void TicketService_GetAllTicketsForThisUser_Returns_ListOfTicketDomainModels()
        {
            //Arrange
            IEnumerable<Ticket> listOfTickets = _listOfTickets;
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(listOfTickets);
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockTicketRepository.Setup(x => x.GetAllUnpaidForSpecificUser(It.IsAny<string>())).Returns(responseTask);
            TicketService ticketService = new TicketService(_mockTicketRepository.Object, _mockUserRepository.Object);
            //Act
            var resultAction = ticketService.GetAllTicketsForThisUser(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            var result = (List<TicketDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(List<TicketDomainModel>));
            Assert.IsInstanceOfType(result[0], typeof(TicketDomainModel));
        }
    }
}
