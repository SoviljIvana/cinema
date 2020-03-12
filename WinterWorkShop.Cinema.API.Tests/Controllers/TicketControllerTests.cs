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
            _createTicketModel.seatModels.Add(_seatDomainModel);
            _ticketResultModel = new TicketResultModel() 
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Ticket = _ticketDomainModel
            };
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


    }
}
