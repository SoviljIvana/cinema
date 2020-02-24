﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly IMovieTagsRepository _movieTagsRepository;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository, IMovieTagsRepository movieTagsRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
            _movieTagsRepository = movieTagsRepository;
        }

        public async Task<IEnumerable<CreateMovieResultModel>> GetAllMoviesWithThisTag(string tag)
        {
            var data = _movieTagsRepository.GetAllMovieTagsForSpecificTag(tag).Result.ToList();
            List<CreateMovieResultModel> result = new List<CreateMovieResultModel>();

            if (data == null)
            {
                result.Add(new CreateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                });
                return result;
            }
            var n = data.Count();


            if (data.Count() == 0)
            {
                result.Add(new CreateMovieResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Movie = new MovieDomainModel
                    {
                        Title = "Movie not found"
                    }
                });
                return result;
            }

            List<ProjectionDomainFilterModel> listProjDomMode = new List<ProjectionDomainFilterModel>();

            foreach (var item in data)
            {
                CreateMovieResultModel model = new CreateMovieResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_SUCCESSFUL,
                    Movie = new MovieDomainModel
                    {
                         Title = item.Movie.Title,
                        Current = item.Movie.Current,
                        Id = item.Id,
                        Year = item.Movie.Year, 
                        Rating = item.Movie.Rating ?? 0
                    }
                };
                result.Add(model);
            }
            return result;

        }

        public IEnumerable<MovieDomainModel> GetCurrentMovies(bool? isCurrent)
        {
            var data = _moviesRepository.GetCurrent();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);
            }

            return result;

        }

        public async Task<MovieDomainModel> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }
           
            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie)
        {
            Movie movieToCreate = new Movie()
            {
                Title = newMovie.Title,
                Current = newMovie.Current,
                Year = newMovie.Year,
                Rating = newMovie.Rating
            };

            var data = _moviesRepository.Insert(movieToCreate);
            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> UpdateMovie(MovieDomainModel updateMovie)
        {

            var item = await _moviesRepository.GetByIdAsync(updateMovie.Id);

            if (item == null)
            {
                return null;
            }

            if (item.Projections != null)
            {
                List<Projection> checkProjection = item.Projections.ToList();

                var dateTimeNow = DateTime.Now;
                foreach (var projection in checkProjection)
                {
                    if (projection.DateTime > dateTimeNow)
                    {
                        return null;
                    }
                }
            }

            Movie movie = new Movie()
            {
                Id = updateMovie.Id,
                Title = updateMovie.Title,
                Current = updateMovie.Current,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating
            };

            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return null;
            }
            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0
            };

            return domainModel;
        }

        public async Task<CreateMovieResultModel> UpdateMovieStatus(MovieDomainModel updateMovie)
        {
            var item = await _moviesRepository.GetByIdAsync(updateMovie.Id);

            if (item == null)
            {
                return null;
            }

            //current 
            var projections = _projectionsRepository.GetAllFromOneMovie(updateMovie.Id).Result;

            if (projections != null)
            {
                
                foreach (var projection in projections)
                {
                    if (projection.DateTime > DateTime.Now)
                    {
                        return new CreateMovieResultModel
                        {
                            IsSuccessful = false,
                            ErrorMessage = Messages.MOVIE_CURRENT_UPDATE_ERROR
                        };
                    }
                }
            }

            Movie movie = new Movie()
            {
                Id = updateMovie.Id,
                Title = updateMovie.Title,
                Current = updateMovie.Current,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating
            };

            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return new CreateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.MOVIE_CURRENT_UPDATE_ERROR
                };
            }
            _moviesRepository.Save();

            CreateMovieResultModel domainModel = new CreateMovieResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Movie = new MovieDomainModel
                {
                    Id = data.Id,
                    Title = data.Title,
                    Current = data.Current,
                    Year = data.Year,
                    Rating = data.Rating ?? 0
                }
            };

            return domainModel;
        }


        public async Task<MovieDomainModel> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0

            };

            return domainModel;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetTopTenMovies()
        {
            var data = await _moviesRepository.GetTopTenMovies();

            if(data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);

            }
            return result;
        }

        public IEnumerable<MovieDomainModel> GetCurrentAndNotCurrentMovies()
        {
            var data = _moviesRepository.GetCurrentAndNotCurrentMovies();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);
            }

            return result;

        }
    }
}
