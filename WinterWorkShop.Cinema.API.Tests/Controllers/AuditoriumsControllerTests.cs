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
    public class AuditoriumsControllerTests
    {
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private CreateAuditoriumModel _createAuditoriumModel;
        private CreateAuditoriumResultModel _createAuditoriumResultModel;
        private AuditoriumResultModel _auditoriumResultModel;
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
            _auditoriumResultModel = new AuditoriumResultModel()
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
        public void AuditoriumController_GetAuditoriumsForCinema_Return_NotFoundObject()
        {
            //Arrange
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = null;
            int expectedStatusCode = 404;

            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(auditoriumDomainModels);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //ACT
            var resultAction = auditoriumsController.GetAuditoriumsForCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void AuditoriumController_GetAuditoriumsForCinema_Return_All_Auditoriums()
        {
            //Arrange
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = _listOfAuditoriumDomainModels;
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAllOfSpecificCinema(It.IsAny<int>())).Returns(auditoriumDomainModels);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //ACT
            var result = auditoriumsController.GetAuditoriumsForCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        [TestMethod]
        public void AuditoriumController_GetAsync_ReturnNotFoundObjectResult()
        {
            //Arrange
            int expectedStatusCode = 404;
            var errorMessageExpected = "Auditorium does not exist.";
            AuditoriumDomainModel auditoriumDomainModel = null;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.GetAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(result, errorMessageExpected);
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void AuditoriumController_Put_Returns_BadRequest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            auditoriumsController.ModelState.AddModelError("key", "Invalid Model State");
            //Act
            var result = auditoriumsController.Put(It.IsAny<int>(), It.IsAny<UpdateAuditoriumModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void AuditoriumController_Put_Returns_Returns_NotFound()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Auditorium does not exist.";
            AuditoriumDomainModel auditoriumDomainModel = null;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Put(It.IsAny<int>(), It.IsAny<UpdateAuditoriumModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)result).ErrorMessage;
            var resultStatusCode = ((ErrorResponseModel)result).StatusCode;
            var resultStatusCodeIntoInt = ((int)resultStatusCode);
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, resultErrorResponseModel);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }

        [TestMethod]
        public void AuditoriumController_Put_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            int expectedStatusCode = 400;
            UpdateAuditoriumModel updateAuditoriumModel = new UpdateAuditoriumModel();
            string expectedErrorMessage = "Inner exception error message.";
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _mockAuditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>())).Throws(dbUpdateException);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Put(It.IsAny<int>(), updateAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void AuditoriumController_Put_Returns_ErrorResponse_AuditoriumUpdateErrorMessage()
        {
            //Arrange
            int expectedStatusCode = 500;
            UpdateAuditoriumModel updateAuditoriumModel = new UpdateAuditoriumModel();
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumResultModel auditoriumResultModel = new AuditoriumResultModel()
            {
                ErrorMessage = "Error happened when auditorium was updating. ",
                IsSuccessful = false
            };
            Task<AuditoriumResultModel> responseTaskFromUpdate = Task.FromResult(auditoriumResultModel);
            _mockAuditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>())).Returns(responseTaskFromUpdate);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Put(It.IsAny<int>(), updateAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((ObjectResult)resultAction).Value;
            var resultErrorResponseModel = ((ErrorResponseModel)resultResponse).ErrorMessage;
            var statusCode = ((ErrorResponseModel)resultResponse).StatusCode;
            var resultStatusCodeIntoInt = ((int)statusCode);
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(auditoriumResultModel.ErrorMessage, resultErrorResponseModel);
            Assert.AreEqual(expectedStatusCode, resultStatusCodeIntoInt);
        }
        [TestMethod]
        public void AuditoriumController_Put_Returns_Accepted()
        {
            //Arrange
            var expectedStatusCode = 202;
            UpdateAuditoriumModel updateAuditoriumModel = new UpdateAuditoriumModel() 
            { 
                cinemaId = 1,
                name = "AuditoriumName",
                numberOfSeats =1,
                seatRows = 1
            };
            
            AuditoriumDomainModel auditoriumDomainModel = _auditoriumDomainModel;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumResultModel auditoriumResultModel = _auditoriumResultModel;
            
            Task<AuditoriumResultModel> responseTaskFromUpdate = Task.FromResult(auditoriumResultModel);
            _mockAuditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>())).Returns(responseTaskFromUpdate);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Put(It.IsAny<int>(), updateAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var resultAuditoriumResultModel = (AuditoriumResultModel)resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultAuditoriumResultModel.IsSuccessful);
            Assert.IsNull(resultAuditoriumResultModel.ErrorMessage);
            Assert.AreEqual(updateAuditoriumModel.name, resultAuditoriumResultModel.Auditorium.Name);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }
        [TestMethod]
        public void AuditoriumController_Delete_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            string expectedErrorMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Throws(dbUpdateException);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void AuditoriumController_Delete_Returns_StatusCode_AUDITORIUM_DELETION_ERROR()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Unable to delete auditorium, please make sure there are no upcoming projections and then try again. ";
            AuditoriumResultModel auditoriumResultModel = new AuditoriumResultModel() 
            {
                Auditorium = null
            };
            Task<AuditoriumResultModel> responseTask = Task.FromResult(auditoriumResultModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(id)).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void AuditoriumController_Delete_Returns_StatusCode_AUDITORIUM_DELETION_ERROR_SuccessfulFalse()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Unable to delete auditorium, please make sure there are no upcoming projections and then try again. ";
            AuditoriumResultModel auditoriumResultModel = new AuditoriumResultModel()
            {
                Auditorium = _auditoriumDomainModel,
                IsSuccessful = false
            };
            Task<AuditoriumResultModel> responseTask = Task.FromResult(auditoriumResultModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(id)).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void AuditoriumController_Delete_Returns_Accepted()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 202;
            AuditoriumResultModel auditoriumResultModel = new AuditoriumResultModel()
            {
                Auditorium = _auditoriumDomainModel,
                IsSuccessful = true,
                ErrorMessage = null,
            };
            Task<AuditoriumResultModel> responseTask = Task.FromResult(auditoriumResultModel);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(id)).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_mockAuditoriumService.Object);
            //Act
            var resultAction = auditoriumsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var resultAuditoriumModel = (AuditoriumResultModel)resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultAuditoriumModel.IsSuccessful);
            Assert.IsNull(resultAuditoriumModel.ErrorMessage);
            Assert.AreEqual(_auditoriumDomainModel.Name, resultAuditoriumModel.Auditorium.Name);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }









    }
}
