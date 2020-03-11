using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
                return NotFound(Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR);
            }

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

            ProjectionResultModel createProjectionResultModel;

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
        public async Task<ActionResult<CreateProjectionFilterResultModel>> FilterByAll(string searchData)
        {
            CreateProjectionFilterResultModel createProjectionFilterResultModel;

            createProjectionFilterResultModel = await _projectionService.FilterAllProjections(searchData);

            if (!createProjectionFilterResultModel.IsSuccessful)
            {
                return NotFound(createProjectionFilterResultModel.ErrorMessage);
            }

            return Ok(createProjectionFilterResultModel);
        }

        /// <summary>
        /// Searches for a projection filtered by movie name
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("moviename/{searchData}")]
        public async Task<ActionResult<CreateProjectionFilterResultModel>> FilterByMovieName(string searchData)
        {
            CreateProjectionFilterResultModel projectionDomainModels;

            projectionDomainModels = await _projectionService.FilterProjectionsByMovieName(searchData);

            if (!projectionDomainModels.IsSuccessful)
            {
                return NotFound(projectionDomainModels.ErrorMessage);
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
        public async Task<ActionResult<CreateProjectionFilterResultModel>> FilterByCinemaName(string searchData)
        {
            CreateProjectionFilterResultModel projectionDomainModels;

            projectionDomainModels = await _projectionService.FilterProjectionsByCinemaName(searchData);

            if (!projectionDomainModels.IsSuccessful)
            {
                return NotFound(projectionDomainModels.ErrorMessage);
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
        public async Task<ActionResult<CreateProjectionFilterResultModel>> FilterByAuditoriumName(string searchData)
        {
            CreateProjectionFilterResultModel projectionDomainModels;

            projectionDomainModels = await _projectionService.FilterProjectionsByAuditoriumName(searchData);

            if (!projectionDomainModels.IsSuccessful)
            {
                return NotFound(projectionDomainModels.ErrorMessage);
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
        public async Task<ActionResult<CreateProjectionFilterResultModel>> FilterByDates(DateTime startDate, DateTime endDate)
        {
            CreateProjectionFilterResultModel projectionDomainModels;

            projectionDomainModels = await _projectionService.FilterProjectionsByDates(startDate, endDate);

            if (!projectionDomainModels.IsSuccessful)
            {
                return NotFound(projectionDomainModels.ErrorMessage);
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
        public async Task<ActionResult> DeleteProjection(Guid id)
        {
            ProjectionResultModel deletedProjection;
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
            return Accepted("auditoriums//" + deletedProjection.Projection.Id, deletedProjection);
        }
    }
}