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
    public class SeatsControllerTests
    {
        private Mock<ISeatService> _mockSeatService;
        private List<RowsDomainModel> _rows;
        private RowsDomainModel _rowsDomainModel;
        private SeatDomainModel _seatDomainModel;
        [TestInitialize]
        public void TestInitialize()
        {
            _seatDomainModel = new SeatDomainModel()
            {
                Id = Guid.NewGuid()
            };
            _rowsDomainModel = new RowsDomainModel()
            {
                SeatsInRow = new List<SeatDomainModel>()
            };
            _rowsDomainModel.SeatsInRow.Add(_seatDomainModel);
            _rows = new List<RowsDomainModel>();
            _rows.Add(_rowsDomainModel);
            _mockSeatService = new Mock<ISeatService>();
        }

        [TestMethod]
        public void SeatsController_GetAllSeatsForSpecificProjection_Returns_NotFoundMessage() 
        {
            //Arrange
            IEnumerable<RowsDomainModel> rowsDomainModels = null;
            Task<IEnumerable<RowsDomainModel>> responseTask = Task.FromResult(rowsDomainModels);
            int expectedStatusCode = 404;

            _mockSeatService = new Mock<ISeatService>();
            _mockSeatService.Setup(x => x.GetAllSeatsForProjection(It.IsAny<Guid>())).Returns(responseTask);
            SeatsController seatsController = new SeatsController(_mockSeatService.Object);
            //Act
            var resultAction = seatsController.GetAllSeatsForSpecificProjection(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }
        [TestMethod]
        public void SeatsController_GetAllSeatsForSpecificProjection_Returns_OkObjectResult_RowsWithSeats()
        {
            //Arrange
            IEnumerable<RowsDomainModel> rowsDomainModels = _rows;
            Task<IEnumerable<RowsDomainModel>> responseTask = Task.FromResult(rowsDomainModels);
            int expectedStatusCode = 200;
            int expectedResultCount = 1;

            _mockSeatService = new Mock<ISeatService>();
            _mockSeatService.Setup(x => x.GetAllSeatsForProjection(It.IsAny<Guid>())).Returns(responseTask);
            SeatsController seatsController = new SeatsController(_mockSeatService.Object);
            //Act
            var resultAction = seatsController.GetAllSeatsForSpecificProjection(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var rows = (List<RowsDomainModel>)result;
            //Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(expectedResultCount, rows.Count);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
            Assert.AreEqual(rows[0].SeatsInRow[0].Id, _rows[0].SeatsInRow[0].Id);
        }
    }
}
