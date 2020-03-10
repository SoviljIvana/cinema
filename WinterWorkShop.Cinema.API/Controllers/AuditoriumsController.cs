using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriumsController : ControllerBase
    {
        private readonly IAuditoriumService _auditoriumService;

        public AuditoriumsController(IAuditoriumService auditoriumservice)
        {
            _auditoriumService = auditoriumservice;
        }

        /// <summary>
        /// Gets all auditoriums
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<AuditoriumDomainModel>>> GetAsync()
        {
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels;

            auditoriumDomainModels = await _auditoriumService.GetAllAsync();

            if (auditoriumDomainModels == null)
            {
                return NotFound(Messages.AUDITORIUM_NOT_FOUND);
            }
            return Ok(auditoriumDomainModels);
        }

        /// <summary>
        /// Adds a new auditorium
        /// </summary>
        /// <param name="createAuditoriumModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AuditoriumDomainModel>> PostAsync(CreateAuditoriumModel createAuditoriumModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = createAuditoriumModel.cinemaId,
                Name = createAuditoriumModel.name
            };

            CreateAuditoriumResultModel createAuditoriumResultModel;

            try
            {
                createAuditoriumResultModel = await _auditoriumService.CreateAuditorium(auditoriumDomainModel, createAuditoriumModel.seatRows, createAuditoriumModel.numberOfSeats);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!createAuditoriumResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = createAuditoriumResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }
            return Created("auditoriums//" + createAuditoriumResultModel.Auditorium.Id, createAuditoriumResultModel);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AuditoriumDomainModel>> GetAsync(int id)
        {
            AuditoriumDomainModel auditorium;

            auditorium = await _auditoriumService.GetAuditoriumByIdAsync(id);

            if (auditorium == null)
            {
                return NotFound(Messages.AUDITORIUM_DOES_NOT_EXIST);
            }

            return Ok(auditorium);
        }

        /// <summary>
        /// Updates an auditorium
        /// </summary>
        /// <param name="id"></param>
        /// <param name="auditoriumModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]UpdateAuditoriumModel auditoriumModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuditoriumDomainModel auditoriumToUpdate;

            auditoriumToUpdate = await _auditoriumService.GetAuditoriumByIdAsync(id);

            if (auditoriumToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

                return NotFound(errorResponse);
            }

            if (auditoriumModel.numberOfSeats != 0)
                auditoriumToUpdate.NumberOfSeats = auditoriumModel.numberOfSeats;

            if (auditoriumModel.seatRows!=0)
                auditoriumToUpdate.SeatRows = auditoriumModel.seatRows;

            auditoriumToUpdate.Name = auditoriumModel.name;
            auditoriumToUpdate.CinemaId = auditoriumModel.cinemaId;

            AuditoriumResultModel auditoriumDomainModel;
            try
            {
                auditoriumDomainModel = await _auditoriumService.UpdateAuditorium(auditoriumToUpdate);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!auditoriumDomainModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_UPDATE_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("auditoriums//" + auditoriumDomainModel.Auditorium.Id, auditoriumDomainModel);

        }
        /// <summary>
        /// Deletes an auditorium if there are no future projections in it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            AuditoriumResultModel deletedAuditorium; 
            try
            {
                deletedAuditorium = await _auditoriumService.DeleteAuditorium(id); 
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);

            }

            if (deletedAuditorium.Auditorium == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DELETION_ERROR, 
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse); 
            }

            if (!deletedAuditorium.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DELETION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("auditoriums//" + deletedAuditorium.Auditorium.Id, deletedAuditorium);

        }


    }
}