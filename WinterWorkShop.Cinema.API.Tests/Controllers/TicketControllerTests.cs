using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class TicketControllerTests
    {
        private Mock<ITicketService> _mockTicketService;
        private TicketResultModel _ticketResultModel;
        private CreateTicketModel _createTicketModel;
        private TicketDomainModel _ticketDomainModel;
        private SeatDomainModel _seatDomainModel;
        private List<TicketDomainModel> _listOfTicketDomainModels;

        [TestInitialize]
        public void TestInitialize()
        {
            _createTicketModel = new CreateTicketModel()
            {
                Id = Guid.NewGuid(),
                ProjectionId = Guid.NewGuid(),
                UserName = "UserName",
                seatModels = new List<SeatDomainModel>()
            };
            _seatDomainModel = new SeatDomainModel()
            {
                Id = Guid.NewGuid()
            };
            _ticketDomainModel = new TicketDomainModel()
            {
                Paid = false,
                Id = Guid.NewGuid()
            };
            _createTicketModel.seatModels.Add(_seatDomainModel);
            _ticketResultModel = new TicketResultModel() 
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Ticket = _ticketDomainModel
            };
            _listOfTicketDomainModels = new List<TicketDomainModel>();
            _listOfTicketDomainModels.Add(_ticketDomainModel);
            _mockTicketService = new Mock<ITicketService>();
        }


        [TestMethod]
        public void TicketsController_CreateTicket_Returns_BadRequest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            CreateTicketModel createTicketModel = _createTicketModel;
            _mockTicketService = new Mock<ITicketService>();
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            ticketsController.ModelState.AddModelError("key", "Invalid Model State");
            //Arrange
            var result = ticketsController.CreateTicket(It.IsAny<CreateTicketModel>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void TicketsController_CreateTicket_Returns_BadRequest_DbUpdateExceptione()
        {
            //Arrange
            TicketDomainModel ticketDomainModel = new TicketDomainModel();
            CreateTicketModel createTicketModel = _createTicketModel;
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.CreateNewTicket(It.IsAny<TicketDomainModel>())).Throws(dbUpdateException);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Arrange
            var resultAction = ticketsController.CreateTicket(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void TicketsController_CreateTicket_Returns_IsSuccessful_False()
        {
            //Arrange
            TicketResultModel ticketResultModel = new TicketResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "errorMessage"
            };
            Task<TicketResultModel> responseTask = Task.FromResult(ticketResultModel);
            TicketDomainModel ticketDomainModel = new TicketDomainModel();
            CreateTicketModel createTicketModel = _createTicketModel;
            int expectedStatusCode = 400;
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.CreateNewTicket(It.IsAny<TicketDomainModel>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Arrange
            var resultAction = ticketsController.CreateTicket(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsNotNull(errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void TicketsController_CreateTicket_Returns_Created_ListOfTickets()
        {
            //Arrange
            int expectedStatusCode = 200;
            TicketResultModel ticketResultModel = _ticketResultModel;
            Task<TicketResultModel> responseTask = Task.FromResult(ticketResultModel);
            TicketDomainModel ticketDomainModel = new TicketDomainModel();
            CreateTicketModel createTicketModel = _createTicketModel;
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.CreateNewTicket(It.IsAny<TicketDomainModel>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Arrange
            var resultAction = ticketsController.CreateTicket(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((OkObjectResult)resultAction).Value;
            var resultTicketResultModel = (List<TicketResultModel>)createdResult;
            //Assert
            Assert.IsNotNull(resultTicketResultModel);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.IsNull(resultTicketResultModel[0].ErrorMessage);
            Assert.IsTrue(resultTicketResultModel[0].IsSuccessful);
        }

        [TestMethod]
        public void TicketsController_GetAllUnpaidTicketsForUser_Returns_ListOfTickets()
        {
            //Arrange
            int expectedStatusCode = 200;

            IEnumerable<TicketDomainModel> ticketDomainModels = _listOfTicketDomainModels;
            Task<IEnumerable<TicketDomainModel>> responseTask = Task.FromResult(ticketDomainModels);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.GetAllTicketsForThisUser(It.IsAny<string>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);

            //Act
            var resultAction = ticketsController.GetAllUnpaidTicketsForUser(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var resultList = (List<TicketDomainModel>)result;
            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void TicketsController_GetAllUnpaidTicketsForUser_Return_NotFoundObject()
        {
            //Arrange
            int expectedStatusCode = 404;
            var expectedErrorMessage = "Error occured while finding ticket, please try again.";

            IEnumerable<TicketDomainModel> ticketDomainModels = null;
            Task<IEnumerable<TicketDomainModel>> responseTask = Task.FromResult(ticketDomainModels);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.GetAllTicketsForThisUser(It.IsAny<string>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);

            //Act
            var resultAction = ticketsController.GetAllUnpaidTicketsForUser(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void TicketsController_Delete_Return_NotFoundObject()
        {
            //Arrange
            string expectedErrorMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.DeleteTicketById(It.IsAny<Guid>())).Throws(dbUpdateException);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Act
            var resultAction = ticketsController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void TicketsController_Delete_Returns_StatusCode_TICKET_DOES_NOT_EXIST()
        {
            //Arrange
            string expectedErrorMessage = "Ticket does not exist. ";
            int expectedStatusCode = 500;
            TicketResultModel ticketResultModel = null;
            Task<TicketResultModel> responseTask = Task.FromResult(ticketResultModel);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.DeleteTicketById(It.IsAny<Guid>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Act
            var resultAction = ticketsController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }
        [TestMethod]
        public void TicketsController_Delete_Returns_StatusCode_TICKET_NOT_FOUND()
        {
            //Arrange
            string expectedErrorMessage = "Error occured while finding ticket, please try again.";
            int expectedStatusCode = 500;
            TicketResultModel ticketResultModel = new TicketResultModel()
            {
                Ticket = null
            };
            Task<TicketResultModel> responseTask = Task.FromResult(ticketResultModel);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.DeleteTicketById(It.IsAny<Guid>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Act
            var resultAction = ticketsController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }
        [TestMethod]
        public void TicketsController_Delete_Returns_Accepted()
        {
            //Arrange
            int expectedStatusCode = 202;

            TicketResultModel ticketResultModel = _ticketResultModel;
            Task<TicketResultModel> responseTask = Task.FromResult(ticketResultModel);
            _mockTicketService = new Mock<ITicketService>();
            _mockTicketService.Setup(x => x.DeleteTicketById(It.IsAny<Guid>())).Returns(responseTask);
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object);
            //Act
            var resultAction = ticketsController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var resultModel = (TicketResultModel) resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(_ticketDomainModel.Id, resultModel.Ticket.Id);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }
    }
}
