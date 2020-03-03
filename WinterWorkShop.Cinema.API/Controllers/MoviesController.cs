using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;
        private readonly IProjectionService _projectionService;
        private readonly ITagService _tagService;

        public MoviesController(ILogger<MoviesController> logger, IMovieService movieService, IProjectionService projectionService, ITagService tagService )
        {
            _logger = logger;
            _movieService = movieService;
            _projectionService = projectionService;
            _tagService = tagService;
        }

        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MovieDomainModel>> GetAsync(Guid id)
        {
            MovieDomainModel movie;

            movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                return NotFound(Messages.MOVIE_DOES_NOT_EXIST);
            }
            return Ok(movie);
        }

        [HttpGet]
        [Route("allForSpecificMovie/{id}")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetProjectionsForSpecificMovie(Guid id)
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;

            projectionDomainModels = await _projectionService.GetAllAsyncForSpecificMovie(id);

            if (projectionDomainModels == null)
            {
                return NotFound(Messages.PROJECTION_SEARCH_NORESULT);
            }
            return Ok(projectionDomainModels);
        }
        //TODO: checkStatusCode
        [HttpGet]
        [Route("search/{searchData}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> SearchByTag(string searchData)
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = await _movieService.GetAllMoviesWithThisTag(searchData);

            if (movieDomainModels == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = Messages.MOVIE_WITH_THIS_DESCRIPTION_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return NotFound(errorResponse);
            }
            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Gets all current movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetCurrent()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;
            try
            {
                movieDomainModels = await _movieService.GetCurrentMovies();
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

            if (movieDomainModels == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return NotFound(errorResponse);
            }

            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Gets all movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("allMovies")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetCurrentAndNotCurrent()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = await _movieService.GetCurrentAndNotCurrentMovies();

            if (movieDomainModels == null)
            {
                return NotFound(Messages.MOVIE_GET_ALL_MOVIES_ERROR);
            }
            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> CreateNewMovieWithHisTags([FromBody]CreateMovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Current = movieModel.Current,
                Rating = movieModel.Rating,
                Title = movieModel.Title,
                Year = movieModel.Year
            };

            MovieCreateTagDomainModel movieCreateTagDomainModel = new MovieCreateTagDomainModel
            {
                Duration = movieModel.Duration,
                tagsForMovieToAdd = movieModel.listOfTags
            };
            
            MovieDomainModel createMovie;
            try
            {
                createMovie = await _movieService.AddMovie(domainModel, movieCreateTagDomainModel);
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

            if (createMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("movies//" + createMovie.Id, createMovie);
        }

        /// <summary>
        /// Updates a movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateMovie(Guid id, [FromBody]UpdateMovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel movieToUpdate;

            movieToUpdate = await _movieService.GetMovieByIdAsync(id);

            if (movieToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

                return NotFound(errorResponse);
            }

            movieToUpdate.Title = movieModel.Title;
            movieToUpdate.Current = movieModel.Current;
            movieToUpdate.Year = movieModel.Year;
            movieToUpdate.Rating = movieModel.Rating;

            CreateMovieResultModel movieDomainModel;
            try
            {
                movieDomainModel = await _movieService.UpdateMovie(movieToUpdate);
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
            if (!movieDomainModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = movieDomainModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("movies//" + movieDomainModel.Movie.Id, movieDomainModel);

        }

        /// <summary>
        /// Delete a movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            DeleteMovieModel deletedMovie;
            try
            {
                deletedMovie = await _movieService.DeleteMovie(id);
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

            if (deletedMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }
            if (deletedMovie.MovieDomainModel == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PROJECTION_IN_FUTURE,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("movies//" + deletedMovie.MovieDomainModel.Id, deletedMovie);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("top")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetTopList() 
        {
           var movies = await _movieService.GetTopTenMovies(); 

            if (movies == null)
            {
                return NotFound(Messages.MOVIE_GET_TOP_10_FAILED_TO_GET);
            }
            return Ok(movies);
        }

        [HttpPut]
        [Route("currentstatus/{id}")]
        public async Task<ActionResult> UpdateMovieCurrentStatus(Guid id)
        {
            MovieDomainModel movieToUpdate;

            movieToUpdate = await _movieService.GetMovieByIdAsync(id);

            if (movieToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

                return NotFound(errorResponse);
            }

            CreateMovieResultModel createMovieResultModel;
            try
            {
                createMovieResultModel = await _movieService.UpdateMovieStatus(id);
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

            if (!createMovieResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createMovieResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("movies//" + createMovieResultModel.Movie.Id, createMovieResultModel.Movie);
        }

        //trebaDaSeTestira
        [HttpGet]
        [Route("allTags")]
        public async Task<ActionResult<TagDomainModel>> GetAllTagsForMovieCreate()
        {
            var data = await _tagService.GetAllTags();
            if (data == null)
            {
                return NotFound(Messages.TAGS_NOT_FOUND);
            }
            return data;   
        }
    }
}
