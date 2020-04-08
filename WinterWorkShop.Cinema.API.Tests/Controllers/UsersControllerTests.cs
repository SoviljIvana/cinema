using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests
    {
        private Mock<IUserService> _mockUsersService;
        private UserDomainModel _userDomainModel;
        private List<UserDomainModel> _listOfUserDomainModel;
        [TestInitialize]
        public void TestInitialize()
        {
            _userDomainModel = new UserDomainModel()
            {
                Id = Guid.NewGuid(),
                FirstName = "First"
            };
            _listOfUserDomainModel = new List<UserDomainModel>();
            _listOfUserDomainModel.Add(_userDomainModel);
            _mockUsersService = new Mock<IUserService>();
        }

        //[TestMethod]
        //public void UsersController_GetAll_Returns_NewEmptyList()
        //{
        //    //Arrange
        //    //Act
        //    //Assert
        //}
        [TestMethod]
        public void UsersController_GetbyIdAsync_Returns_NotFound_MessageNotFound()
        {
            //Arrange
            UserDomainModel userDomainModel = null;
            var expectedErrorMessage = "User does not exist.";
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModel);
            int expectedStatusCode = 404;
            _mockUsersService = new Mock<IUserService>();
            _mockUsersService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UsersController usersController = new UsersController(_mockUsersService.Object);
            //Act
            var resultAction = usersController.GetbyIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, expectedErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void UsersController_GetbyIdAsync_Returns_OkObjectResult()
        {
            //Arrange
            UserDomainModel userDomainModel = _userDomainModel;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModel);
            int expectedStatusCode = 200;
            _mockUsersService = new Mock<IUserService>();
            _mockUsersService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UsersController usersController = new UsersController(_mockUsersService.Object);
            //Act
            var resultAction = usersController.GetbyIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var user = (UserDomainModel)result;
            //Assert
            Assert.IsNotNull(user);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(userDomainModel.Id, user.Id);
        }
        [TestMethod]
        public void UsersController_GetbyUserNameAsync_Returns_NotFound_MessageNotFound()
        {
            //Arrange
            UserDomainModel userDomainModel = null;
            var expectedErrorMessage = "User does not exist.";
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModel);
            int expectedStatusCode = 404;
            _mockUsersService = new Mock<IUserService>();
            _mockUsersService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(responseTask);
            UsersController usersController = new UsersController(_mockUsersService.Object);
            //Act
            var resultAction = usersController.GetbyUserNameAsync(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, expectedErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void UsersController_GetbyUserNameAsync_Returns_OkObjectResult()
        {
            //Arrange
            UserDomainModel userDomainModel = _userDomainModel;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModel);
            int expectedStatusCode = 200;
            _mockUsersService = new Mock<IUserService>();
            _mockUsersService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(responseTask);
            UsersController usersController = new UsersController(_mockUsersService.Object);
            //Act
            var resultAction = usersController.GetbyUserNameAsync(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var user = (UserDomainModel)result;
            //Assert
            Assert.IsNotNull(user);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(userDomainModel.Id, user.Id);
        }
    }
}
