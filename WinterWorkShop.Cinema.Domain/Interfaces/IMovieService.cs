﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// Get all movies by current parameter
        /// </summary>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        Task<IEnumerable<MovieDomainModel>> GetCurrentMovies();

        Task<IEnumerable<MovieDomainModel>> GetCurrentAndNotCurrentMovies();

        /// <summary>
        /// Get a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MovieDomainModel> GetMovieByIdAsync(Guid id);

        /// <summary>
        /// Adds new movie to DB
        /// </summary>
        /// <param name="newMovie"></param>
        /// <returns></returns>
        Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie, MovieCreateTagDomainModel movieCreateTagDomainModel);

        /// <summary>
        /// Update a movie to DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CreateMovieResultModel> UpdateMovie(MovieDomainModel updateMovie);

        /// <summary>
        /// Delete a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DeleteMovieModel> DeleteMovie(Guid id);
        Task<IEnumerable<MovieDomainModel>> GetTopTenMovies();
        Task<CreateMovieResultModel> UpdateMovieStatus(Guid id);
        Task<IEnumerable<MovieDomainModel>> GetAllMoviesWithThisTag(string tag);

    }
}
