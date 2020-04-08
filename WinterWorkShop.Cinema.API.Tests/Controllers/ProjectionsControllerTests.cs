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
    public class ProjectionsControllerTests
    {
        private Mock<IProjectionService> _projectionService;
        

        [TestMethod]
        public void GetAsync_Return_All_Projections()
        {
            //Arrange
            List<ProjectionDomainModel> projectionsDomainModelsList = new List<ProjectionDomainModel>();
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            projectionsDomainModelsList.Add(projectionDomainModel);
            IEnumerable<ProjectionDomainModel> projectionDomainModels = projectionsDomainModelsList;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.AreEqual(projectionDomainModel.Id, projectionDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            var expectedMessage = "Error occured while getting all projections, please try again.";
            IEnumerable<ProjectionDomainModel> projectionDomainModels = null;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedStatusCode = 404;
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var resultAction = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - false
        // return Created
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_True_Projection() 
        {
            //Arrange
            int expectedStatusCode = 201;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            ProjectionResultModel createProjectionResultModel = new ProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AuditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = true
            };
            Task<ProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);


            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var projectionDomainModel = (ProjectionDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(projectionDomainModel);
            Assert.AreEqual(createProjectionModel.MovieId, projectionDomainModel.MovieId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - throw DbUpdateException
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Projection()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            ProjectionResultModel createProjectionResultModel = new ProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AuditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = true
            };
            Task<ProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Throws(dbUpdateException);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_False_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new projection, please try again.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            ProjectionResultModel createProjectionResultModel = new ProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AuditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = false,
                ErrorMessage = Messages.PROJECTION_CREATION_ERROR,
            };
            Task<ProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);


            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 0
            };

            _projectionService = new Mock<IProjectionService>();
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            projectionsController.ModelState.AddModelError("key","Invalid Model State");

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ProjectionDate_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projection time cannot be in past.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 0
            };

            _projectionService = new Mock<IProjectionService>();
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault(nameof(createProjectionModel.ProjectionTime));
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void ProjectionController_FilterByAll_ReturnNotFound()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Please enter search parameter"
            };

            var expectedErrorMessage = "Please enter search parameter";
            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterAllProjections(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByAll(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (NotFoundObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, (result).Value);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public void ProjectionController_FilterByAll_Return_CreateProjectionFilterResultModel()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = new List<ProjectionDomainFilterModel>()
            };
            ProjectionDomainFilterModel projectionDomainFilterModel = new ProjectionDomainFilterModel()
            {
                AuditoriumName = "AuditoriumName"
            };
            createProjectionFilterResultModel.Projections.Add(projectionDomainFilterModel);

            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterAllProjections(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByAll(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var okObjectResultmodel = ((OkObjectResult)resultAction).Value;
            var resultModel = (CreateProjectionFilterResultModel)okObjectResultmodel;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(projectionDomainFilterModel.AuditoriumName, resultModel.Projections[0].AuditoriumName);
        }

        [TestMethod]
        public void ProjectionController_FilterByMovieName_ReturnNotFound()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Please enter search parameter"
            };

            var expectedErrorMessage = "Please enter search parameter";
            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByMovieName(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByMovieName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (NotFoundObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, (result).Value);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public void ProjectionController_FilterByMovieName_Return_CreateProjectionFilterResultModel()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = new List<ProjectionDomainFilterModel>()
            };
            ProjectionDomainFilterModel projectionDomainFilterModel = new ProjectionDomainFilterModel()
            {
                AuditoriumName = "AuditoriumName"
            };
            createProjectionFilterResultModel.Projections.Add(projectionDomainFilterModel);

            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByMovieName(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByMovieName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var okObjectResultmodel = ((OkObjectResult)resultAction).Value;
            var resultModel = (CreateProjectionFilterResultModel)okObjectResultmodel;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(projectionDomainFilterModel.AuditoriumName, resultModel.Projections[0].AuditoriumName);
        }
        [TestMethod]
        public void ProjectionController_FilterByCinemaName_ReturnNotFound()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Please enter search parameter"
            };

            var expectedErrorMessage = "Please enter search parameter";
            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByCinemaName(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByCinemaName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (NotFoundObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, (result).Value);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public void ProjectionController_FilterByCinemaName_Return_CreateProjectionFilterResultModel()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = new List<ProjectionDomainFilterModel>()
            };
            ProjectionDomainFilterModel projectionDomainFilterModel = new ProjectionDomainFilterModel()
            {
                AuditoriumName = "AuditoriumName"
            };
            createProjectionFilterResultModel.Projections.Add(projectionDomainFilterModel);

            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByCinemaName(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByCinemaName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var okObjectResultmodel = ((OkObjectResult)resultAction).Value;
            var resultModel = (CreateProjectionFilterResultModel)okObjectResultmodel;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(projectionDomainFilterModel.AuditoriumName, resultModel.Projections[0].AuditoriumName);
        }
        [TestMethod]
        public void ProjectionController_FilterByAuditoriumName_ReturnNotFound()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Please enter search parameter"
            };

            var expectedErrorMessage = "Please enter search parameter";
            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByAuditoriumName(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByAuditoriumName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (NotFoundObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, (result).Value);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public void ProjectionController_FilterByAuditoriumName_Return_CreateProjectionFilterResultModel()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = new List<ProjectionDomainFilterModel>()
            };
            ProjectionDomainFilterModel projectionDomainFilterModel = new ProjectionDomainFilterModel()
            {
                AuditoriumName = "AuditoriumName"
            };
            createProjectionFilterResultModel.Projections.Add(projectionDomainFilterModel);

            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByAuditoriumName(It.IsAny<string>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByAuditoriumName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var okObjectResultmodel = ((OkObjectResult)resultAction).Value;
            var resultModel = (CreateProjectionFilterResultModel)okObjectResultmodel;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(projectionDomainFilterModel.AuditoriumName, resultModel.Projections[0].AuditoriumName);
        }
        [TestMethod]
        public void ProjectionController_FilterByDates_ReturnNotFound()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = false,
                ErrorMessage = "Please enter search parameter"
            };

            var expectedErrorMessage = "Please enter search parameter";
            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (NotFoundObjectResult)resultAction;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedErrorMessage, (result).Value);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public void ProjectionController_FilterByDates_Return_CreateProjectionFilterResultModel()
        {
            //Arrange
            CreateProjectionFilterResultModel createProjectionFilterResultModel = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = new List<ProjectionDomainFilterModel>()
            };
            ProjectionDomainFilterModel projectionDomainFilterModel = new ProjectionDomainFilterModel()
            {
                AuditoriumName = "AuditoriumName"
            };
            createProjectionFilterResultModel.Projections.Add(projectionDomainFilterModel);

            Task<CreateProjectionFilterResultModel> reponseTask = Task.FromResult(createProjectionFilterResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjectionsByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.FilterByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var okObjectResultmodel = ((OkObjectResult)resultAction).Value;
            var resultModel = (CreateProjectionFilterResultModel)okObjectResultmodel;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(projectionDomainFilterModel.AuditoriumName, resultModel.Projections[0].AuditoriumName);
        }
        [TestMethod]
        public void ProjectionController_DeleteProjection_Return_Accepted()
        {
            //Arrange
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel()
            {
                AuditoriumName = "AuditoriumName"
            };
            ProjectionResultModel projectionResultModel = new ProjectionResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = projectionDomainModel
            };
            
            Task<ProjectionResultModel> reponseTask = Task.FromResult(projectionResultModel);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.DeleteProjection(It.IsAny<Guid>())).Returns(reponseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.DeleteProjection(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (AcceptedResult)resultAction;
            var acceptedResultmodel = ((AcceptedResult)resultAction).Value;
            var resultModel = (ProjectionResultModel)acceptedResultmodel;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(202, result.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.IsTrue(resultModel.IsSuccessful);
            Assert.IsNull(resultModel.ErrorMessage);
            Assert.AreEqual(projectionDomainModel.AuditoriumName, resultModel.Projection.AuditoriumName);
        }
        [TestMethod]
        public void ProjectionController_DeleteProjection_Return_DbUpdateException()
        {
            //Arrange
            string expectedErrorMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.DeleteProjection(It.IsAny<Guid>())).Throws(dbUpdateException);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            //Act
            var resultAction = projectionsController.DeleteProjection(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)resultAction;
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;
            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, resultResponse.StatusCode);
        }
    }
}
