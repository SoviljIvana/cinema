using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class AuditoriumControllerTests
    {
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private CreateAuditoriumModel _createAuditoriumModel;
        private CreateAuditoriumResultModel _createAuditoriumResultModel;
        private List<AuditoriumDomainModel> _listOfAuditoriumDomainModels;

        [TestInitialize]
        public void TestInitialize()
        {
            _auditoriumDomainModel = new AuditoriumDomainModel()
            {
                Id = 1,
                CinemaId = 1,
                Name = "AuditoriumName",
                NumberOfSeats = 1,
                SeatRows = 1,
                SeatsList = new List<SeatDomainModel>()
            };
            _createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                name = "AuditoriumName",
                numberOfSeats = 1,
                seatRows = 1
            };
            _createAuditoriumResultModel = new CreateAuditoriumResultModel()
            {
                Auditorium = _auditoriumDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            _listOfAuditoriumDomainModels = new List<AuditoriumDomainModel>();
            _listOfAuditoriumDomainModels.Add(_auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
        }

        [TestMethod]
        public void AuditoriumController_GetAsync_Return_All_Auditoriums()
        {
            //Arrange
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = _listOfAuditoriumDomainModels;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriumDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //ACT
            var result = auditoriumsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(auditoriumDomainModelResultList);
            Assert.AreEqual(expectedResultCount, auditoriumDomainModelResultList.Count);
            Assert.AreEqual(auditoriumDomainModel.Id, auditoriumDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void AuditoriumController_GetAsync_Return_NotFoundObject()
        {
            //Arrange
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = null;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriumDomainModels);
            int expectedStatusCode = 404;

            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //ACT
            var resultAction = auditoriumsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void AuditoriumController_PostAsync_Returns_BadRequest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            CreateAuditoriumModel createAuditoriumModel = _createAuditoriumModel;
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            auditoriumsController.ModelState.AddModelError("key", "Invalid Model State");
            //Act
            var result = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
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
        public void AuditoriumController_PostAsync_Returns_BadRequest_DbUpdateExceptione()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            CreateAuditoriumModel createAuditoriumModel = _createAuditoriumModel;
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);

            //Act
            var resultAction = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
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
        public void AuditoriumController_PostAsync_Returns_IsSuccessful_False()
        {
            //Arrange
            int expectedStatusCode = 400;
            CreateAuditoriumResultModel createAuditoriumResultModel = new CreateAuditoriumResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "errorMessage"
            };

            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);

            CreateAuditoriumModel createAuditoriumModel = _createAuditoriumModel;
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);

            //Act
            var resultAction = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
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
        public void AuditoriumController_PostAsync_Returns_Created_createAuditoriumResultModel()
        {
            //Arrange
            int expectedStatusCode = 201;
            CreateAuditoriumResultModel createAuditoriumResultModel = _createAuditoriumResultModel;
            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);
            CreateAuditoriumModel createAuditoriumModel = _createAuditoriumModel;
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);

            //Act
            var resultAction = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
            var createdResult = ((CreatedResult)resultAction).Value;
            var resultCreateAuditoriumResultModel = (CreateAuditoriumResultModel)createdResult;
            //Assert
            Assert.IsNotNull(resultCreateAuditoriumResultModel);
            Assert.IsInstanceOfType(resultAction, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)resultAction).StatusCode);
            Assert.IsNull(resultCreateAuditoriumResultModel.ErrorMessage);
            Assert.IsTrue(resultCreateAuditoriumResultModel.IsSuccessful);
            Assert.AreEqual(createAuditoriumResultModel.Auditorium.Id, resultCreateAuditoriumResultModel.Auditorium.Id);
        }
        [TestMethod]
        public void AuditoriumController_GetAsync_ReturnOkObjectResult()
        {
            //Arrange
            int expectedStatusCode = 200;
            
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.GetAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var returnModel = (AuditoriumDomainModel)result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(auditoriumDomainModel.Id, returnModel.Id);

        }






    }
}
