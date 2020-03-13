using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
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
        private readonly ITagRepository _tagRepository;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository, IMovieTagsRepository movieTagsRepository, ITicketRepository ticketRepository, ITagRepository tagRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
            _movieTagsRepository = movieTagsRepository;
            _ticketRepository = ticketRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetAllMoviesWithThisTag(string tag)
        {
            List<MovieDomainModel> result = new List<MovieDomainModel>();

            List<string> listOfString = tag.Split(' ').ToList();

            var allMovieTags = await _movieTagsRepository.GetAll();
            var allMovies = await _moviesRepository.GetAllWithMovieTags();

            List<Movie> listOfFilms = new List<Movie>();
            List<Movie> listOfFilmByTitle = new List<Movie>();

            foreach (var stringData in listOfString)
            {
                List<MovieTag> movieTags;
                movieTags = allMovieTags.Where(y => y.Tag.Name.Contains(stringData) || y.Tag.Type.Contains(stringData)).ToList();
                if (movieTags.Count != 0)
                {
                    if (listOfFilms.Count == 0)
                    {
                        foreach (var movieTag in movieTags)
                        {
                            var movie = allMovies.SingleOrDefault(g => g.Id.Equals(movieTag.MovieId));
                            listOfFilms.Add(movie);
                        }
                    }
                    else
                    {
                        var listOfFilmsForCheck = new List<Movie>();
                        listOfFilmsForCheck = listOfFilms;

                        for (int j = 0; j < listOfFilmsForCheck.Count; j++)
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
                        }
                    }
                }
                else
                {
                    foreach (var movieTitle in listOfString)
                    {
                        List<Movie> movieTitles;

                        movieTitles = allMovies.Where(y => y.Title.Contains(movieTitle)).ToList();
                        if (movieTitles.Count != 0)
                        {
                            if (listOfFilmByTitle.Count == 0)
                            {
                                foreach (var movie in movieTitles)
                                {
                                    listOfFilmByTitle.Add(movie);
                                }
                            }
                            else
                            {
                                var listOfFilmsForCheck = new List<Movie>();
                                listOfFilmsForCheck = listOfFilmByTitle;

                                for (int j = 0; j < listOfFilmsForCheck.Count; j++)
                                {
                                    int numberOfNotMatching = 0;
                                    for (int i = 0; i < movieTitles.Count; i++)
                                    {
                                        if (!movieTitles[i].Id.Equals(listOfFilmsForCheck[j].Id))
                                        {
                                            numberOfNotMatching = numberOfNotMatching + 1;
                                        }
                                        if (numberOfNotMatching == movieTitles.Count)
                                        {
                                            listOfFilmByTitle.Remove(listOfFilmsForCheck[j]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            if (listOfFilms.Count() == 0 && listOfFilmByTitle.Count == 0)
            {
                return null;
            }

            if (listOfFilmByTitle != null && listOfFilmByTitle.Count > 0)
            {
                foreach (var item in listOfFilmByTitle)
                {
                    MovieDomainModel model = new MovieDomainModel
                    {
                        Title = item.Title,
                        Current = item.Current,
                        Id = item.Id,
                        Year = item.Year,
                        Rating = item.Rating ?? 0,
                        listOfProjections = new List<ProjectionDomainModel>()
                    };
                    var projectionsForThisMovie = _projectionsRepository.GetAllFromOneMovie(item.Id);

                    foreach (var projection in projectionsForThisMovie)
                    {
                        model.listOfProjections.Add(new ProjectionDomainModel()
                        {
                            Id = projection.Id,
                            ProjectionTimeString = projection.DateTime.ToString("hh:mm tt"),
                            AuditoriumName = projection.Auditorium.Name
                        });

                    }
                    result.Add(model);
                }
            }

            foreach (var item in listOfFilms)
            {
                TagsMovieModel tagsMovieModel = new TagsMovieModel();

                MovieDomainModel model = new MovieDomainModel
                {
                    Title = item.Title,
                    Current = item.Current,
                    Id = item.Id,
                    Year = item.Year,
                    Rating = item.Rating ?? 0,
                    listOfProjections = new List<ProjectionDomainModel>(),
                    tagsMovieModel = tagsMovieModel

                };
                var TagsForMovie = _movieTagsRepository.GetAllForSpecificMovie(item.Id).Result.ToList();

                foreach (var tagData in TagsForMovie)
                {
                    if (tagData.Tag.Type.Equals("director"))
                    {
                        tagsMovieModel.Directores = tagsMovieModel.Directores + " " + tagData.Tag.Name;
                    }
                    if (tagData.Tag.Type.Equals("genre"))
                    {
                        tagsMovieModel.Generes = tagsMovieModel.Generes + " " + tagData.Tag.Name;
                    }
                    if (tagData.Tag.Type.Equals("duration"))
                    {
                        tagsMovieModel.Duration = tagsMovieModel.Duration + " " + tagData.Tag.Name;
                    }
                    if (tagData.Tag.Type.Equals("aword"))
                    {
                        tagsMovieModel.Awards = tagsMovieModel.Awards + " " + tagData.Tag.Name;
                    }
                    if (tagData.Tag.Type.Equals("language"))
                    {
                        tagsMovieModel.Languages = tagsMovieModel.Languages + " " + tagData.Tag.Name;
                    }
                    if (tagData.Tag.Type.Equals("state"))
                    {
                        tagsMovieModel.States = tagsMovieModel.States + " " + tagData.Tag.Name;
                    }
                    if (tagData.Tag.Type.Equals("actor"))
                    {
                        tagsMovieModel.Actores = tagsMovieModel.Actores + " " + tagData.Tag.Name;
                    }
                }
                model.tagsMovieModel = tagsMovieModel;
                var projectionsForThisMovie = _projectionsRepository.GetAllFromOneMovie(item.Id);

                foreach (var projection in projectionsForThisMovie)
                {
                    model.listOfProjections.Add(new ProjectionDomainModel()
                    {
                        Id = projection.Id,
                        ProjectionTimeString = projection.DateTime.ToString("hh:mm tt"),
                        AuditoriumName = projection.Auditorium.Name
                    });

                }
                result.Add(model);
            }
            return result;
        }

        public async Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie, MovieCreateTagDomainModel movieCreateTagDomainModel)
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

            if (movieCreateTagDomainModel.Duration > 0)
            {
                var durationIntToString = movieCreateTagDomainModel.Duration.ToString();
                Tag durationTag = new Tag
                {
                    Name = durationIntToString,
                    Type = "duration",
                };

                var newTagAdded = _tagRepository.Insert(durationTag);

                MovieTag movieTag = new MovieTag()
                {
                    MovieId = data.Id,
                    Tag = newTagAdded
                };

                _movieTagsRepository.Insert(movieTag);
            }

            if (movieCreateTagDomainModel.tagsForMovieToAdd != null && movieCreateTagDomainModel.tagsForMovieToAdd.Count > 0)
            {
                foreach (var item in movieCreateTagDomainModel.tagsForMovieToAdd)
                {
                    var findTag = _tagRepository.GetByIdName(item);
                    if (findTag != null)
                    {
                        MovieTag moviTagToAdd = new MovieTag
                        {
                            MovieId = data.Id,
                            TagId = findTag.Id
                        };
                        _movieTagsRepository.Insert(moviTagToAdd);
                    }
                }
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

        public async Task<CreateMovieResultModel> UpdateMovie(MovieDomainModel updateMovie)
        {
            var item = await _moviesRepository.GetByIdAsync(updateMovie.Id);

            var projections = _projectionsRepository.GetAllFromOneMovie(updateMovie.Id);

            if (projections != null)
            {
                if (item.Current.Equals(true) && updateMovie.Current.Equals(false))
                {
                    List<Projection> checkProjection = projections.ToList();

                    var dateTimeNow = DateTime.Now;
                    foreach (var projection in checkProjection)
                    {
                        if (projection.DateTime > dateTimeNow)
                        {
                            return new CreateMovieResultModel
                            {
                                IsSuccessful = false,
                                ErrorMessage = Messages.MOVIE_CURRENT_TO_NOT_CURRENT_UPDATE_ERROR
                            };
                        }
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

            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Movie = new MovieDomainModel()
                {
                    Id = data.Id,
                    Title = data.Title,
                    Current = data.Current,
                    Year = data.Year,
                    Rating = data.Rating ?? 0
                },

            };
            return createMovieResultModel;
        }

        public async Task<CreateMovieResultModel> UpdateMovieStatus(Guid id)
        {
            var item = await _moviesRepository.GetByIdAsync(id);

            if (item == null)
            {
                return null;
            }
            //ako je current=true i ima projekciju, ne moze biti false
            var projections = _projectionsRepository.GetAllFromOneMovie(id);
            if (item.Current)
            {
                if (projections != null)
                {
                    foreach (var projection in projections)
                    {
                        if (projection.DateTime > DateTime.Now)
                        {
                            return new CreateMovieResultModel
                            {
                                IsSuccessful = false,
                                ErrorMessage = Messages.MOVIE_CURRENT_TO_NOT_CURRENT_UPDATE_ERROR
                            };
                        }
                    }
                }
            }

            var currentNewValue = !item.Current;

            Movie movie = new Movie()
            {
                Id = item.Id,
                Title = item.Title,
                Current = currentNewValue,
                Year = item.Year,
                Rating = item.Rating
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
                    Current = data.Current
                }
            };
            return domainModel;
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

        public async Task<DeleteMovieModel> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }
            var projectionsForDelete = _projectionsRepository.GetAllFromOneMovie(id);
            foreach (var projectionForDelete in projectionsForDelete)
            {
                if (projectionForDelete.DateTime < DateTime.Now)
                {
                    _projectionsRepository.Delete(projectionForDelete.Id);
                }
                else
                {
                    return new DeleteMovieModel();
                }
                var ticketsForDelete = _ticketRepository.GetAllForSpecificProjection(projectionForDelete.Id);
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

            if (data == null)
            {
                return null;
            }

            var oskarId = _movieTagsRepository.GetOskarId().Id;
            double rating = 0;
            List<Movie> listaMovie = new List<Movie>();
            foreach (var item in data)
            {
                listaMovie.Add(item);
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            List<Movie> resultOrder = new List<Movie>();

            MovieDomainModel model;
            for (int i = 0; i < listaMovie.Count(); i++)
            {
                if (listaMovie[i].Rating == rating)
                {
                    var hasOskarListAnswer = listaMovie[i].MovieTags.Select(x => x.TagId.Equals(oskarId));

                    if (hasOskarListAnswer.Contains(true))
                    {
                        var firstElement = listaMovie[i];
                        var secondElement = listaMovie[i - 1];
                        resultOrder.Remove(secondElement);
                        resultOrder.Add(firstElement);
                        resultOrder.Add(secondElement);

                    }
                    resultOrder.Add(listaMovie[i]);

                }
                else
                {
                    resultOrder.Add(listaMovie[i]);
                    rating = listaMovie[i].Rating ?? 0;
                }
            }

            var finalOrder = resultOrder.Take(10);

            foreach (var item in finalOrder)
            {
                TagsMovieModel tagsMovieModel = new TagsMovieModel();

                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    listOfProjections = new List<ProjectionDomainModel>(),
                    tagsMovieModel = new TagsMovieModel(),
                };
                var projectionsForThisMovie = _projectionsRepository.GetAllFromOneMovie(item.Id);

                foreach (var projection in projectionsForThisMovie)
                {
                    model.listOfProjections.Add(new ProjectionDomainModel()
                    {
                        Id = projection.Id,
                        ProjectionTimeString = projection.DateTime.ToString("hh:mm tt"),
                        AuditoriumName = projection.Auditorium.Name
                    });

                }
                var TagsForMovie = _movieTagsRepository.GetAllForSpecificMovie(item.Id).Result.ToList();

                foreach (var tag in TagsForMovie)
                {
                    if (tag.Tag.Type.Equals("director"))
                    {
                        tagsMovieModel.Directores = tagsMovieModel.Directores + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("genre"))
                    {
                        tagsMovieModel.Generes = tagsMovieModel.Generes + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("duration"))
                    {
                        tagsMovieModel.Duration = tagsMovieModel.Duration + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("aword"))
                    {
                        tagsMovieModel.Awards = tagsMovieModel.Awards + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("language"))
                    {
                        tagsMovieModel.Languages = tagsMovieModel.Languages + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("state"))
                    {
                        tagsMovieModel.States = tagsMovieModel.States + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("actor"))
                    {
                        tagsMovieModel.Actores = tagsMovieModel.Actores + " " + tag.Tag.Name;
                    }
                }
                model.tagsMovieModel = tagsMovieModel;
                result.Add(model);

            }
            return result;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetCurrentMoviesWithoutProjections()
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
                TagsMovieModel tagsMovieModel = new TagsMovieModel();

                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    listOfProjections = new List<ProjectionDomainModel>(),
                    tagsMovieModel = new TagsMovieModel()
                };
                var TagsForMovie = _movieTagsRepository.GetAllForSpecificMovie(item.Id).Result.ToList();

                foreach (var tag in TagsForMovie)
                {
                    if (tag.Tag.Type.Equals("director"))
                    {
                        tagsMovieModel.Directores = tagsMovieModel.Directores + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("genre"))
                    {
                        tagsMovieModel.Generes = tagsMovieModel.Generes + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("duration"))
                    {
                        tagsMovieModel.Duration = tagsMovieModel.Duration + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("aword"))
                    {
                        tagsMovieModel.Awards = tagsMovieModel.Awards + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("language"))
                    {
                        tagsMovieModel.Languages = tagsMovieModel.Languages + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("state"))
                    {
                        tagsMovieModel.States = tagsMovieModel.States + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("actor"))
                    {
                        tagsMovieModel.Actores = tagsMovieModel.Actores + " " + tag.Tag.Name;
                    }
                }
                model.tagsMovieModel = tagsMovieModel;
                IEnumerable<Projection> lista = new List<Projection>();

                var projectionsForThisMovie = _projectionsRepository.GetAllFromOneMovie(item.Id).ToList();
                if (projectionsForThisMovie == null || projectionsForThisMovie.Count==0)
                {
                    result.Add(model);
                }

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
                TagsMovieModel tagsMovieModel = new TagsMovieModel();

                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    listOfProjections = new List<ProjectionDomainModel>(),
                    tagsMovieModel = new TagsMovieModel(),
                };
                IEnumerable<Projection> lista = new List<Projection>();
                var projectionsForThisMovie = _projectionsRepository.GetAllFromOneMovie(item.Id);
                foreach (var projection in projectionsForThisMovie)
                {
                    model.listOfProjections.Add(new ProjectionDomainModel()
                    {
                        Id = projection.Id,
                        ProjectionTimeString = projection.DateTime.ToString("MM/dd/yyyy HH:mm"),
                        AuditoriumName = projection.Auditorium.Name
                    });

                }
                var TagsForMovie = _movieTagsRepository.GetAllForSpecificMovie(item.Id).Result.ToList();

                foreach (var tag in TagsForMovie)
                {
                    if (tag.Tag.Type.Equals("director"))
                    {
                        tagsMovieModel.Directores = tagsMovieModel.Directores + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("genre"))
                    {
                        tagsMovieModel.Generes = tagsMovieModel.Generes + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("duration"))
                    {
                        tagsMovieModel.Duration = tagsMovieModel.Duration + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("aword"))
                    {
                        tagsMovieModel.Awards = tagsMovieModel.Awards + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("language"))
                    {
                        tagsMovieModel.Languages = tagsMovieModel.Languages + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("state"))
                    {
                        tagsMovieModel.States = tagsMovieModel.States + " " + tag.Tag.Name;
                    }
                    if (tag.Tag.Type.Equals("actor"))
                    {
                        tagsMovieModel.Actores = tagsMovieModel.Actores + " " + tag.Tag.Name;
                    }
                }
                model.tagsMovieModel = tagsMovieModel;
                result.Add(model);
            }

            return result;

        }
        public async Task<IEnumerable<MovieDomainModel>> GetCurrentMoviesForToday()
        {
            var data = await _moviesRepository.GetCurrentMoviesWithProjectionsForToday();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                TagsMovieModel tagsMovieModel = new TagsMovieModel();

                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    listOfProjections = new List<ProjectionDomainModel>(),
                    tagsMovieModel = new TagsMovieModel()

                };

                IEnumerable<Projection> lista = new List<Projection>();

                var dayToday = DateTime.Now.DayOfYear;
                var yearToday = DateTime.Now.Year;

                var projectionsForThisMovieAll = _projectionsRepository.GetAllFromOneMovie(item.Id);
                var projectionsForThisMovieToday = projectionsForThisMovieAll.Where(x => x.DateTime.DayOfYear == dayToday && x.DateTime.Year == yearToday).ToList();
                if (projectionsForThisMovieToday != null && projectionsForThisMovieToday.Count>0)
                {
                    foreach (var projection in projectionsForThisMovieToday)
                    {
                        model.listOfProjections.Add(new ProjectionDomainModel()
                        {
                            Id = projection.Id,
                            ProjectionTimeString = projection.DateTime.ToString("hh:mm tt"),
                            AuditoriumName = projection.Auditorium.Name,
                            CinemaName = projection.Auditorium.Cinema.Name
                        });
                    }
                    var TagsForMovie = _movieTagsRepository.GetAllForSpecificMovie(item.Id).Result.ToList();
                    foreach (var tag in TagsForMovie)
                    {
                        if (tag.Tag.Type.Equals("director"))
                        {
                            tagsMovieModel.Directores = tagsMovieModel.Directores + " " + tag.Tag.Name;
                        }
                        if (tag.Tag.Type.Equals("genre"))
                        {
                            tagsMovieModel.Generes = tagsMovieModel.Generes + " " + tag.Tag.Name;
                        }
                        if (tag.Tag.Type.Equals("duration"))
                        {
                            tagsMovieModel.Duration = tagsMovieModel.Duration + " " + tag.Tag.Name;
                        }
                        if (tag.Tag.Type.Equals("aword"))
                        {
                            tagsMovieModel.Awards = tagsMovieModel.Awards + " " + tag.Tag.Name;
                        }
                        if (tag.Tag.Type.Equals("language"))
                        {
                            tagsMovieModel.Languages = tagsMovieModel.Languages + " " + tag.Tag.Name;
                        }
                        if (tag.Tag.Type.Equals("state"))
                        {
                            tagsMovieModel.States = tagsMovieModel.States + " " + tag.Tag.Name;
                        }
                        if (tag.Tag.Type.Equals("actor"))
                        {
                            tagsMovieModel.Actores = tagsMovieModel.Actores + " " + tag.Tag.Name;
                        }
                    }
                    model.tagsMovieModel = tagsMovieModel;
                    result.Add(model);
                }

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
