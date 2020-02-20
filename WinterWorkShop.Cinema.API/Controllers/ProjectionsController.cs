﻿using System;
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

        [HttpGet]
        [Route("FilterByText/{searchData}")]
        public async Task<ActionResult<IEnumerable<Projection>>> FilterByAll(string searchData)
        {
            IEnumerable<ProjectionDomainFilterModel> projectionDomainModels;
            projectionDomainModels = await _projectionService.FilterAllProjections(searchData);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainFilterModel>();
            }
            return Ok(projectionDomainModels);
        }

        [HttpGet]
        [Route("moviename/{searchData}")]
        public async Task<ActionResult<IEnumerable<Projection>>> FilterByMovieName(string searchData)
        {
            IEnumerable<ProjectionDomainFilterModel> projectionDomainModels;
            projectionDomainModels = await _projectionService.FilterProjectionsByMovieName(searchData);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainFilterModel>();
            }
            return Ok(projectionDomainModels);
        }

        [HttpGet]
        [Route("cinemaname/{searchData}")]
        public async Task<ActionResult<IEnumerable<Projection>>> FilterByCinemaName(string searchData)
        {
            IEnumerable<ProjectionDomainFilterModel> projectionDomainModels;
            projectionDomainModels = await _projectionService.FilterProjectionsByCinemaName(searchData);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainFilterModel>();
            }
            return Ok(projectionDomainModels);
        }

        [HttpGet]
        [Route("auditname/{searchData}")]
        public async Task<ActionResult<IEnumerable<Projection>>> FilterByAuditoriumName(string searchData)
        {
            IEnumerable<ProjectionDomainFilterModel> projectionDomainModels;
            projectionDomainModels = await _projectionService.FilterProjectiondByAuditoriumName(searchData);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainFilterModel>();
            }
            return Ok(projectionDomainModels);
        }

        [HttpGet]
        [Route("dates/{startDate},{endDate}")]
        public async Task<ActionResult<IEnumerable<Projection>>> FilterByDates(DateTime startDate, DateTime endDate)
        {
            IEnumerable<ProjectionDomainFilterModel> projectionDomainModels;
            projectionDomainModels = await _projectionService.FilterProjectiondByDates(startDate, endDate);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainFilterModel>();
            }
            return Ok(projectionDomainModels);
        }
    }
}