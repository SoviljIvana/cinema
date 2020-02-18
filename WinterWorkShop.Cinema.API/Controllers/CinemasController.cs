using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CinemasController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        /// <summary>
        /// Gets all cinemas
        /// </summary>
        /// <returns>List of cinemas</returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<CinemaDomainModel>>> GetAsync()
        {
            IEnumerable<CinemaDomainModel> cinemaDomainModels;

            cinemaDomainModels = await _cinemaService.GetAllAsync();

            if (cinemaDomainModels == null)
            {
                cinemaDomainModels = new List<CinemaDomainModel>();
            }
            return Ok(cinemaDomainModels);
        }

        //Add a new cinema
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Post([FromBody]CreateCinemaModel cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Name = cinemaModel.Name
            };

            CinemaDomainModel createCinema;

            try
            {
                createCinema = await _cinemaService.AddCinema(domainModel);
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
            return Created("cinema//" + createCinema.Id, createCinema);
        }

        //Gets cinema by id
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CinemaDomainModel>> GetAsync(int id)
        {
            CinemaDomainModel cinema;

            cinema = await _cinemaService.GetCinemaByIdAsync(id);

            if (cinema == null)
            {
                return NotFound(Messages.CINEMA_DOES_NOT_EXIST);
            }

            return Ok(cinema);
        }

        /// <summary>
        /// Delete a cinema by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            CinemaDomainModel deletedCinema;
            try
            {
                deletedCinema = await _cinemaService.DeleteCinema(id);
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

            if (deletedCinema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("cinemas//" + deletedCinema.Id, deletedCinema);
        }

        /// <summary>
        /// Updates a cinema
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cinemaModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]CreateCinemaModel cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel cinemaToUpdate;

            cinemaToUpdate = await _cinemaService.GetCinemaByIdAsync(id);

            if (cinemaToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            cinemaToUpdate.Name = cinemaModel.Name;

            CinemaDomainModel cinemaDomainModel;
            try
            {
                cinemaDomainModel = await _cinemaService.UpdateCinema(cinemaToUpdate);
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

            return Accepted("movies//" + cinemaDomainModel.Id, cinemaDomainModel);

        }
    }
}
