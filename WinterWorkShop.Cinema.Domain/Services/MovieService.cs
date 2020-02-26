using System;
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
        private readonly ITicketRepository _ticketRepository;
        
        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository, IMovieTagsRepository movieTagsRepository, ITicketRepository ticketRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
            _movieTagsRepository = movieTagsRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<CreateMovieResultModel>> GetAllMoviesWithThisTag(string tag)
        {
            List<CreateMovieResultModel> result = new List<CreateMovieResultModel>();

            List<string> listOfString = tag.Split(' ').ToList();

            var allMovieTags = await _movieTagsRepository.GetAll();
            var allMovies = await _moviesRepository.GetAllWithMovieTags();

            List<Movie> listOfFilms = new List<Movie>();

            foreach (var stringData in listOfString)
            {
                var movieTags = allMovieTags.Where(y => y.Tag.Name.Contains(stringData) || y.Tag.Type.Contains(stringData)).ToList();
                
                if (listOfFilms.Count == 0)
                {
                    foreach (var movieTag in movieTags)
                    {
                        var movie = allMovies.SingleOrDefault(g => g.Id.Equals(movieTag.MovieId));
                        listOfFilms.Add(movie);
                    }
                    if (listOfFilms == null)
                    {
                        result.Add(new CreateMovieResultModel
                        {
                            IsSuccessful = false,
                            ErrorMessage = Messages.MOVIE_WITH_THIS_DESCRIPTION_DOES_NOT_EXIST
                        });
                        return result;
                    }
                }
                else
                {
                    var listOfFilmsForCheck = new List<Movie>();
                    listOfFilmsForCheck = listOfFilms;

                    for (int j = 0; j <listOfFilmsForCheck.Count; j++)
                    {
                        int numberOfNotMatching = 0;
                        for (int i = 0; i < movieTags.Count; i++)
                        {
                            if (!movieTags[i].MovieId.Equals(listOfFilmsForCheck[j].Id))
                            {
                                numberOfNotMatching = numberOfNotMatching + 1;
                            }
                            if (numberOfNotMatching == movieTags.Count)
                            {
                                listOfFilms.Remove(listOfFilmsForCheck[j]);
                            }
                        }

                        if (listOfFilms == null)
                        {
                            result.Add(new CreateMovieResultModel
                            {
                                IsSuccessful = false,
                                ErrorMessage = Messages.MOVIE_WITH_THIS_DESCRIPTION_DOES_NOT_EXIST
                            });
                            return result;
                        }
                    }
                }
            }

            if (listOfFilms == null)
            {
                result.Add(new CreateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.MOVIE_WITH_THIS_DESCRIPTION_DOES_NOT_EXIST
                });
                return result;
            }
            var n = listOfFilms.Count();


            if (listOfFilms.Count() == 0)
            {
                result.Add(new CreateMovieResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.MOVIE_WITH_THIS_DESCRIPTION_DOES_NOT_EXIST,
                    Movie = new MovieDomainModel
                    {
                        Title = "Movie not found"
                    }
                });
                return result;
            }

            foreach (var item in listOfFilms)
            {
                CreateMovieResultModel model = new CreateMovieResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.MOVIE_SEARCH_SUCCESSFUL,
                    Movie = new MovieDomainModel
                    {
                         Title = item.Title,
                        Current = item.Current,
                        Id = item.Id,
                        Year = item.Year, 
                        Rating = item.Rating ?? 0
                    }
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


        public async Task<DeleteMovieModel> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            var projectionsForDelete = await _projectionsRepository.GetAllFromOneMovie(id);
            foreach (var projectionForDelete in projectionsForDelete)
            {
                if (projectionForDelete.DateTime<DateTime.Now)
                {
                    _projectionsRepository.Delete(projectionForDelete.Id);
                }
                else
                {
                    return new DeleteMovieModel
                    {
                        //ispraviti messeges na -projection in future
                        ErrorMessage = Messages.PROJECTION_IN_PAST,
                        IsSuccessful = false
                    };
                }
                var ticketsForDelete = await _ticketRepository.GetAllForSpecificProjection(projectionForDelete.Id);
                foreach (var ticketForDelete in ticketsForDelete)
                {
                    _ticketRepository.Delete(ticketForDelete.Id);
                }
            }

            var movieTags = await _movieTagsRepository.GetAllForSpecificMovie(id);
            foreach (var movieTag in movieTags)
            {
                _movieTagsRepository.Delete(movieTag.Id);
            }

            _moviesRepository.Save();

            DeleteMovieModel deleteMovieModel = new DeleteMovieModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                MovieDomainModel = new MovieDomainModel
                {
                    Id = data.Id,
                    Title = data.Title,
                    Current = data.Current,
                    Year = data.Year,
                    Rating = data.Rating ?? 0
                }
            };

            return deleteMovieModel;
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

        public async Task<IEnumerable<MovieDomainModel>> GetCurrentMovies()
        {
            var data = await _moviesRepository.GetCurrent();

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

        public async Task<IEnumerable<MovieDomainModel>> GetCurrentAndNotCurrentMovies()
        {
            var data = await _moviesRepository.GetCurrentAndNotCurrentMovies();

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
