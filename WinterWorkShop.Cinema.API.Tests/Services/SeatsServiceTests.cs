using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
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
        private Mock<ISeatsRepository> _mockSeatRepository;
        private Mock<ITicketRepository> _mockTicketRepository;
        [TestInitialize]
        public void TestInitialize()
        {
            _seat = new Seat()
            {
                Id = Guid.NewGuid()
            };
            _seatDomainModel = new SeatDomainModel()
            {
                Id = _seat.Id
            };
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
        public void SeatsController()
        {
            //Arrange
            //Act
            //Assert
        }
    }
}
