using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }
        //TODO: ovo se nigde ne koristi
        /// <summary>
        /// Gets all seats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetAsync()
        {
            IEnumerable<SeatDomainModel> seatDomainModels;
            
            seatDomainModels = await _seatService.GetAllAsync();

            if (seatDomainModels == null)
            {
                seatDomainModels = new List<SeatDomainModel>();
            }

            return Ok(seatDomainModels);
        }


        //TODO: prebaciti u drugi kontroler!
        /// <summary>
        /// Returns all seats for a specific projection
        /// </summary>
        /// <param name="projectionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allForProjection/{projectionId}")]
        public async Task<ActionResult<IEnumerable<RowsDomainModel>>> GetAllSeatsForSpecificProjection(Guid projectionId)
        {
            IEnumerable<RowsDomainModel> seatDomainModels;


            seatDomainModels = await _seatService.GetAllSeatsForProjection(projectionId);

            //if (seatDomainModels == null)
            //{
            //    seatDomainModels = new List<SeatDomainModel>();
            //}

            return Ok(seatDomainModels);
        }
    }


}
