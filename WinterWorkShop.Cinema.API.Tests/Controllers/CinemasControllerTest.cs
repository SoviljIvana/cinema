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
    public class CinemasControllerTest
    {
        private Mock<ICinemaService> _cinemaService;

        [TestMethod]
        public void GetAsync_Return_All_Cinemas()
        {
            //Arrange
            List<CinemaDomainModel> cinemaDomainModelsList = new List<CinemaDomainModel>();
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "NewName"
            };
            cinemaDomainModelsList.Add(cinemaDomainModel);
            IEnumerable<CinemaDomainModel> cinemaDomainModels = cinemaDomainModelsList;
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
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelsList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void GetAsync_Return_NewList() 
        {
            IEnumerable<CinemaDomainModel> cinemaDomainModels = null;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);

            int expectedResultCount = 0;
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
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        //[TestMethod]
        //public void PostAsync_Create_createCinemaResultModel_IsSuccessful_True_Cinema()
        //{
        //    //Arrange
        //    int expectedStatusCode = 201;

        //    CreateCinemaModel createCinemaModel = new CreateCinemaModel()
        //    {
        //        Name = "NewName"
        //    };

        //    CreateCinemaResultModel createCinemaResultModel = new CreateCinemaResultModel
        //    {
        //        Cinema = new CinemaDomainModel
        //        {
        //            Id = 1,
        //            Name = createCinemaModel.Name
        //        },
        //        IsSuccessful = true, 
        //    };

        //}

    }
}
