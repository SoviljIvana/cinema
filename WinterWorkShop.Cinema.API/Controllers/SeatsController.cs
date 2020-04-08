using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;
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

        /// <summary>
        /// Returns all seats for a specific projection
        /// </summary>
        /// <param name="projectionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allForProjection/{projectionId}")]
        public async Task<ActionResult<IEnumerable<RowsDomainModel>>> GetAllSeatsForSpecificProjection(Guid projectionId)
        {
            IEnumerable<RowsDomainModel> rowsDomainModels;

            rowsDomainModels = await _seatService.GetAllSeatsForProjection(projectionId);

            if (rowsDomainModels == null)
            {
                return NotFound(Messages.SEAT_GET_ALL_SEATS_ERROR);
            }

            return Ok(rowsDomainModels);
        }
    }


}
