using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectionsController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionsController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetAsync()
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;

            projectionDomainModels = await _projectionService.GetAllAsync();

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Returns all projections for a specific movie by movieID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allForSpecificMovie/{id}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetAsyncForSpecificMovie(Guid id)
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;

            projectionDomainModels = await _projectionService.GetAllAsyncForSpecificMovie(id);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            //return Ok(projectionDomainModels + "There is not projections for this movie");
            return Ok(projectionDomainModels);

        }

        /// <summary>
        /// Adds a new projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("")]
        public async Task<ActionResult<ProjectionDomainModel>> PostAsync(CreateProjectionModel projectionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (projectionModel.ProjectionTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(projectionModel.ProjectionTime), Messages.PROJECTION_IN_PAST);
                return BadRequest(ModelState);
            }

            ProjectionDomainModel domainModel = new ProjectionDomainModel
            {
                AuditoriumId = projectionModel.AuditoriumId,
                MovieId = projectionModel.MovieId,
                ProjectionTime = projectionModel.ProjectionTime
            };

            CreateProjectionResultModel createProjectionResultModel;

            try
            {
                createProjectionResultModel = await _projectionService.CreateProjection(domainModel);
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

            if (!createProjectionResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createProjectionResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("projections//" + createProjectionResultModel.Projection.Id, createProjectionResultModel.Projection);
        }

        /// <summary>
        /// Searches for a projections without filter 
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("FilterByText/{searchData}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainFilterModel>>> FilterByAll(string searchData)
        {
            IEnumerable<CreateProjectionFilterResultModel> projectionDomainModels;
            List<CreateProjectionFilterResultModel> list = new List<CreateProjectionFilterResultModel>();

            projectionDomainModels = await _projectionService.FilterAllProjections(searchData);

            list = projectionDomainModels.ToList();

            if (!list[0].IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = list[0].ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Searches for a projection filtered by movie name
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("moviename/{searchData}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainFilterModel>>> FilterByMovieName(string searchData)
        {
            IEnumerable<CreateProjectionFilterResultModel> projectionDomainModels;
            List<CreateProjectionFilterResultModel> list = new List<CreateProjectionFilterResultModel>();

            projectionDomainModels = await _projectionService.FilterProjectionsByMovieName(searchData);

            list = projectionDomainModels.ToList();

            if (!list[0].IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = list[0].ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Searches for a projection filtered by cinema
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("cinemaname/{searchData}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainFilterModel>>> FilterByCinemaName(string searchData)
        {
            IEnumerable<CreateProjectionFilterResultModel> projectionDomainModels;
            List<CreateProjectionFilterResultModel> list = new List<CreateProjectionFilterResultModel>();

            projectionDomainModels = await _projectionService.FilterProjectionsByCinemaName(searchData);

            list = projectionDomainModels.ToList();

            if (!list[0].IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = list[0].ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Searches for a projection filtered by auditorium
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("auditname/{searchData}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainFilterModel>>> FilterByAuditoriumName(string searchData)
        {
            IEnumerable<CreateProjectionFilterResultModel> projectionDomainModels;
            List<CreateProjectionFilterResultModel> list = new List<CreateProjectionFilterResultModel>();

            projectionDomainModels = await _projectionService.FilterProjectionsByAuditoriumName(searchData);

            list = projectionDomainModels.ToList();

            if (!list[0].IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = list[0].ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Searches for a projection filtered by start and end date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("dates/{startDate},{endDate}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainFilterModel>>> FilterByDates(DateTime startDate, DateTime endDate)
        {
            IEnumerable<CreateProjectionFilterResultModel> projectionDomainModels;
            List<CreateProjectionFilterResultModel> list = new List<CreateProjectionFilterResultModel>();

            projectionDomainModels = await _projectionService.FilterProjectionsByDates(startDate, endDate);

            list = projectionDomainModels.ToList();

            if (!list[0].IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = list[0].ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Deletes a specific projection if it has no projections in the future
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            CreateProjectionResultModel deletedProjection;
            try
            {
                deletedProjection = await _projectionService.DeleteProjection(id);
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

            if (deletedProjection == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }
            return Accepted("auditoriums//" + deletedProjection.Projection.Id, deletedProjection);

        }
    }
}