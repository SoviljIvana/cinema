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
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class CinemasControllerTests
    {
        private CinemaDomainModel _cinemaDomainModel;
        private CreateCinemaDomainModel _createCinemaDomainModel;
        private Mock<ICinemaService> _cinemaService;
        private List<CinemaDomainModel> _cinemaDomainModelsList;
        private CreateCinemaWithAuditoriumAndSeatsModel _createCinemaWithAuditoriumAndSeatsModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinemaDomainModel = new CinemaDomainModel()
            {
                Id = 1,
                Name = "NewName"
            };
            _createCinemaDomainModel = new CreateCinemaDomainModel()
            {
                CinemaName = "CinemaName",
                listOfAuditoriums = new List<AuditoriumDomainModel>()
            };
            _createCinemaWithAuditoriumAndSeatsModel = new CreateCinemaWithAuditoriumAndSeatsModel()
            {
                CinemaName = "CinemaName",
                listOfAuditoriums = new List<CreateAuditoriumModel>()
            };
            _cinemaDomainModelsList = new List<CinemaDomainModel>();
            _cinemaDomainModelsList.Add(_cinemaDomainModel);
            _cinemaService = new Mock<ICinemaService>();
        }

        [TestMethod]
        public void CinemasController_GetAsync_Return_All_Cinemas()
        {
            //Arrange
            CinemaDomainModel cinemaDomainModel = _cinemaDomainModel;
            IEnumerable<CinemaDomainModel> cinemaDomainModels = _cinemaDomainModelsList;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //ACT
            var result = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void CinemasController_GetAsync_Return_NotFoundObject() 
        {
            IEnumerable<CinemaDomainModel> cinemaDomainModels = null;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);

            int expectedStatusCode = 404;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //ACT
            var resultAction = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
                
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void CinemasController_PostWithAuditoriumsAndSeats_Returns_BadRequest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            cinemasController.ModelState.AddModelError("key", "Invalid Model State");
            //Act
            var result = cinemasController.PostWithAuditoriumsAndSeats(It.IsAny<CreateCinemaWithAuditoriumAndSeatsModel>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
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
        public void CinemasController_PostWithAuditoriumsAndSeats_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            CreateCinemaWithAuditoriumAndSeatsModel cinemaWithAuditoriumAndSeatsModel =
                _createCinemaWithAuditoriumAndSeatsModel;
            
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinemaWithAuditoriumsAndSeats(It.IsAny<CreateCinemaDomainModel>()))
                .Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.PostWithAuditoriumsAndSeats(cinemaWithAuditoriumAndSeatsModel).ConfigureAwait(false).GetAwaiter()
                .GetResult();
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
        public void CinemasController_PostWithAuditoriumsAndSeats_Returns_IsSuccessful_False()
        {
            //Arrange
            CinemaResultModel cinemaResultModel = new CinemaResultModel()
            {
                ErrorMessage = "errorMessage",
                IsSuccessful = false

            };
            CreateCinemaWithAuditoriumAndSeatsModel cinemaWithAuditoriumAndSeatsModel =
                _createCinemaWithAuditoriumAndSeatsModel;
            Task<CinemaResultModel> reponseTask = Task.FromResult(cinemaResultModel);
            int expectedStatusCode = 400;
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinemaWithAuditoriumsAndSeats(It.IsAny<CreateCinemaDomainModel>()))
                .Returns(reponseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.PostWithAuditoriumsAndSeats(cinemaWithAuditoriumAndSeatsModel).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(cinemaResultModel.ErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void CinemasController_PostWithAuditoriumsAndSeats_Returns_CinemaResultModel()
        {
            //Arrange
            CinemaResultModel cinemaResultModel = new CinemaResultModel()
            {
                IsSuccessful = true,
                Cinema = _cinemaDomainModel

            };
            CreateCinemaWithAuditoriumAndSeatsModel cinemaWithAuditoriumAndSeatsModel =
                _createCinemaWithAuditoriumAndSeatsModel;
            Task<CinemaResultModel> reponseTask = Task.FromResult(cinemaResultModel);
            int expectedStatusCode = 201;
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinemaWithAuditoriumsAndSeats(It.IsAny<CreateCinemaDomainModel>()))
                .Returns(reponseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.PostWithAuditoriumsAndSeats(cinemaWithAuditoriumAndSeatsModel).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            var createdResult = ((CreatedResult)resultAction).Value;
            var resultCinemaResultModel = (CinemaResultModel)createdResult;
            //Assert
            Assert.IsNotNull(resultCinemaResultModel);
            Assert.IsInstanceOfType(resultAction, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)resultAction).StatusCode);
            Assert.IsNull(resultCinemaResultModel.ErrorMessage);
            Assert.IsTrue(resultCinemaResultModel.IsSuccessful);
            Assert.IsInstanceOfType(resultCinemaResultModel, typeof(CinemaResultModel));
        }

        [TestMethod]
        public void CinemasController_GetAsync_Returns_ErrorMessageNotFound()
        {
            //Arrange
            var expectedErrorMessage = "Cinema does not exist.";
            var expectedStatusCode = 404;
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.GetAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
            var result = (NotFoundObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedErrorMessage, result.Value);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }
        [TestMethod]
        public void CinemasController_GetAsync_Returns_OkObjectResult()
        {
            //Arrange
            var expectedStatusCode = 200;
            CinemaDomainModel cinemaDomainModel = _cinemaDomainModel;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.GetAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult().Result;
            var result = (OkObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemasController_Delete_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            string expectedErrorMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            //Assert
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
        public void CinemasController_Delete_Returns_StatusCode_MessageCinemaNotFound()
        {
            //Arrange
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Unable to find cinema, please try again. ";
            CinemaResultModel cinemaResultModel = new CinemaResultModel()
            {
                Cinema = null
            };
            Task<CinemaResultModel> responseTask = Task.FromResult(cinemaResultModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
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
        public void CinemasController_Delete_Returns_StatusCode_Message_DeleteCinemaEror_ProjectionInFuture()
        {
            //Arrange
            int expectedStatusCode = 500;
            string expectedErrorMessage = "Unable to delete auditorium, please make sure there are no upcoming projections and then try again. ";
            CinemaResultModel cinemaResultModel = new CinemaResultModel()
            {
                Cinema = _cinemaDomainModel,
                IsSuccessful = false
            };
            Task<CinemaResultModel> responseTask = Task.FromResult(cinemaResultModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
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
        public void CinemasController_Delete_Returns_Accepted()
        {
            //Arrange
            int expectedStatusCode = 202;
            CinemaResultModel cinemaResultModel = new CinemaResultModel()
            {
                Cinema = _cinemaDomainModel,
                IsSuccessful = true,
                ErrorMessage = null
            };
            Task<CinemaResultModel> responseTask = Task.FromResult(cinemaResultModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var resultCinemaModel = (CinemaResultModel)resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultCinemaModel.IsSuccessful);
            Assert.IsNull(resultCinemaModel.ErrorMessage);
            Assert.AreEqual(_cinemaDomainModel.Name, resultCinemaModel.Cinema.Name);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }

        [TestMethod]
        public void CinemasController_Put_Returns_BadRequeest_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _cinemaService = new Mock<ICinemaService>();
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            cinemasController.ModelState.AddModelError("key", "Invalid Model State");
            //Act
            var result = cinemasController.Put(It.IsAny<int>(), It.IsAny<CreateCinemaModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_Put_Returns_Returns_NotFound()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Cinema does not exist.";
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Put(It.IsAny<int>(), It.IsAny<CreateCinemaModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_Put_Returns_BadRequest_DbUpdateException()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedErrorMessage = "Inner exception error message.";
            CinemaDomainModel cinemaDomainModel = _cinemaDomainModel;
            CreateCinemaModel createCinemaModel = new CreateCinemaModel()
            {
                Name = "newName"
            };
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _cinemaService.Setup(x => x.UpdateCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Put(It.IsAny<int>(), createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_Put_Returns_Accepted()
        {
            //Arrange
            var expectedStatusCode = 202;
            CinemaDomainModel cinemaDomainModel = _cinemaDomainModel;
            CinemaDomainModel newCinema = _cinemaDomainModel;
            CreateCinemaModel createCinemaModel = new CreateCinemaModel()
            {
                Name = "newName"
            };
            newCinema.Name = createCinemaModel.Name;

            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            Task<CinemaDomainModel> responseTaskUpdatedCinema = Task.FromResult(newCinema);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            _cinemaService.Setup(x => x.UpdateCinema(It.IsAny<CinemaDomainModel>())).Returns(responseTaskUpdatedCinema);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            //Act
            var resultAction = cinemasController.Put(It.IsAny<int>(), createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((AcceptedResult)resultAction).Value;
            var statusCode = ((AcceptedResult)resultAction).StatusCode;
            var cinemaDomainModelResult = (CinemaDomainModel)resultResponse;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.AreEqual(createCinemaModel.Name, cinemaDomainModelResult.Name);
            Assert.AreEqual(expectedStatusCode, statusCode);
        }



    }
}
