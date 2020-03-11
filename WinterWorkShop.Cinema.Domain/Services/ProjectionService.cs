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
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatsRepository _seatsRepository;

        public ProjectionService(IProjectionsRepository projectionsRepository,
                                ITicketRepository ticketRepository, ISeatsRepository seatsRepository)
        {
            _projectionsRepository = projectionsRepository;
            _ticketRepository = ticketRepository;
            _seatsRepository = seatsRepository;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> GetAllAsync()
        {
            var data = await _projectionsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AuditoriumName = item.Auditorium.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> GetAllAsyncForSpecificMovie(Guid id)
        {
            var data = _projectionsRepository.GetAllFromOneMovie(id);
            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();

            if (data == null)
            {
                return null;
            }

            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTimeString = item.DateTime.ToString("hh:mm tt"),
                    MovieTitle = item.Movie.Title,
                    AuditoriumName = item.Auditorium.Name
                };
                List < Seat > listaSeat = new List<Seat>();
                var seatsForThisAuditorium = await _seatsRepository.GetAllOfSpecificAuditoriumForProjection(model.AuditoriumId);
                var row = 0;
                var seatPerRow = 0;
                foreach (var seatInAuditorium in seatsForThisAuditorium)
                {
                    listaSeat.Add(seatInAuditorium);
                }
                foreach (var seat in listaSeat)
                {
                    row = seat.Row;
                    seatPerRow = seat.Number;
                }
                model.NumOFRows = row;
                model.NumOFSeatsPerRow = seatPerRow;
                result.Add(model);
            }

            return result;
        }


        public async Task<ProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.DateTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.DateTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new ProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME
                };
            }

            var newProjection = new Data.Projection
            {
                MovieId = domainModel.MovieId,
                AuditoriumId = domainModel.AuditoriumId,
                DateTime = domainModel.ProjectionTime
            };

            var insertedProjection = _projectionsRepository.Insert(newProjection);

            if (insertedProjection == null)
            {
                return new ProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            ProjectionResultModel result = new ProjectionResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = insertedProjection.Id,
                    AuditoriumId = insertedProjection.AuditoriumId,
                    MovieId = insertedProjection.MovieId,
                    ProjectionTime = insertedProjection.DateTime
                }
            };

            return result;
        }

        public async Task<CreateProjectionFilterResultModel> FilterAllProjections(string searchData)
        {
            var data = await _projectionsRepository.FilterAllProjections(searchData);

            CreateProjectionFilterResultModel result = new CreateProjectionFilterResultModel();
            List<ProjectionDomainFilterModel> projectionDomainFilterModels = new List<ProjectionDomainFilterModel>();
            result.Projections = new List<ProjectionDomainFilterModel>();

            if (data == null)
            {
                return new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                };
            }
            var n = data.Count();

            if (data.Count() == 0)
            {

                result = new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projections = new List<ProjectionDomainFilterModel>()

                };
                result.Projections.Add(new ProjectionDomainFilterModel
                {
                    AuditoriumName = "Auditorium not found",
                    MovieTitle = "Movie not found.",
                    CinemaName = "Cinema not found",
                });
                return result;
            }

            foreach (var item in data)
            {
                ProjectionDomainFilterModel projection = new ProjectionDomainFilterModel()
                {
                        Id = item.Id,
                        AuditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                };
                projectionDomainFilterModels.Add(projection);
            }

            result = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = projectionDomainFilterModels
            };

            return result;
        }

        public async Task<CreateProjectionFilterResultModel> FilterProjectionsByMovieName(string searchData)
        {
            var data = await _projectionsRepository.FilterProjectionsByMovieTitle(searchData);

            CreateProjectionFilterResultModel result = new CreateProjectionFilterResultModel();
            List<ProjectionDomainFilterModel> projectionDomainFilterModels = new List<ProjectionDomainFilterModel>();
            result.Projections = new List<ProjectionDomainFilterModel>();

            if (data == null)
            {
                return new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                };
            }
            var n = data.Count();

            if (data.Count() == 0)
            {

                result = new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projections = new List<ProjectionDomainFilterModel>()

                };
                result.Projections.Add(new ProjectionDomainFilterModel
                {
                    AuditoriumName = "Auditorium not found",
                    MovieTitle = "Movie not found.",
                    CinemaName = "Cinema not found",
                });
                return result;
            }

            foreach (var item in data)
            {
                ProjectionDomainFilterModel projection = new ProjectionDomainFilterModel()
                {
                        Id = item.Id,
                        AuditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                };
                projectionDomainFilterModels.Add(projection);
            }

            result = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = projectionDomainFilterModels
            };

            return result;

        }

        public async Task<CreateProjectionFilterResultModel> FilterProjectionsByCinemaName(string searchData)
        {
            var data = await _projectionsRepository.FilterProjectionsByCinemaName(searchData);

            CreateProjectionFilterResultModel result = new CreateProjectionFilterResultModel();
            List<ProjectionDomainFilterModel> projectionDomainFilterModels = new List<ProjectionDomainFilterModel>();
            result.Projections = new List<ProjectionDomainFilterModel>();

            if (data == null)
            {
                return new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                };
            }
            var n = data.Count();

            if (data.Count() == 0)
            {

                result = new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projections = new List<ProjectionDomainFilterModel>()

                };
                result.Projections.Add(new ProjectionDomainFilterModel
                {
                    AuditoriumName = "Auditorium not found",
                    MovieTitle = "Movie not found.",
                    CinemaName = "Cinema not found",
                });
                return result;
            }

            foreach (var item in data)
            {
                ProjectionDomainFilterModel projection = new ProjectionDomainFilterModel()
                {
                    Id = item.Id,
                    AuditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                projectionDomainFilterModels.Add(projection);
            }

            result = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = projectionDomainFilterModels
            };

            return result;

        }

        public async Task<CreateProjectionFilterResultModel> FilterProjectionsByAuditoriumName(string searchData)
        {
            var data = await _projectionsRepository.FilterProjectionsByAuditoriumName(searchData);

            CreateProjectionFilterResultModel result = new CreateProjectionFilterResultModel();
            List<ProjectionDomainFilterModel> projectionDomainFilterModels = new List<ProjectionDomainFilterModel>();
            result.Projections = new List<ProjectionDomainFilterModel>();

            if (data == null)
            {
                return new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                };
            }
            var n = data.Count();

            if (data.Count() == 0)
            {

                result = new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projections = new List<ProjectionDomainFilterModel>()

                };
                result.Projections.Add(new ProjectionDomainFilterModel
                {
                    AuditoriumName = "Auditorium not found",
                    MovieTitle = "Movie not found.",
                    CinemaName = "Cinema not found",
                });
                return result;
            }

            foreach (var item in data)
            {
                ProjectionDomainFilterModel projection = new ProjectionDomainFilterModel()
                {
                    Id = item.Id,
                    AuditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                projectionDomainFilterModels.Add(projection);
            }

            result = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = projectionDomainFilterModels
            };

            return result;
        }

        public async Task<CreateProjectionFilterResultModel> FilterProjectionsByDates(DateTime startDate, DateTime endDate)
        {
            var data = await _projectionsRepository.FilterProjectionsByDates(startDate, endDate);

            CreateProjectionFilterResultModel result = new CreateProjectionFilterResultModel();
            List<ProjectionDomainFilterModel> projectionDomainFilterModels = new List<ProjectionDomainFilterModel>();
            result.Projections = new List<ProjectionDomainFilterModel>();

            if (data == null)
            {
                return new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                };
            }
            var n = data.Count();

            if (data.Count() == 0)
            {

                result = new CreateProjectionFilterResultModel()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projections = new List<ProjectionDomainFilterModel>()

                };
                result.Projections.Add(new ProjectionDomainFilterModel
                {
                    AuditoriumName = "Auditorium not found",
                    MovieTitle = "Movie not found.",
                    CinemaName = "Cinema not found",
                });
                return result;
            }

            foreach (var item in data)
            {
                ProjectionDomainFilterModel projection = new ProjectionDomainFilterModel()
                {
                    Id = item.Id,
                    AuditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                projectionDomainFilterModels.Add(projection);
            }

            result = new CreateProjectionFilterResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projections = projectionDomainFilterModels
            };

            return result;
        }

        public async Task<ProjectionResultModel> DeleteProjection(Guid id)
        {

            var existingProjections = _projectionsRepository.GetProjectionById(id);
            var ticketsInProjection = _ticketRepository.GetAllForSpecificProjection(id);

            if (existingProjections == null)
            {
                ProjectionResultModel errorModel = new ProjectionResultModel
                {
                    ErrorMessage = Messages.PROJECTION_IN_FUTURE,
                    IsSuccessful = true
                };
                return errorModel;
            }

            if (existingProjections.DateTime > DateTime.Now)
            {
                ProjectionResultModel errorModel = new ProjectionResultModel
                {
                    ErrorMessage = Messages.PROJECTION_IN_FUTURE,
                    IsSuccessful = false,
                    Projection = new ProjectionDomainModel
                    {
                        AuditoriumId = existingProjections.AuditoriumId,
                        Id = existingProjections.Id,
                        MovieId = existingProjections.MovieId,
                        AuditoriumName = existingProjections.Auditorium.Name, 
                        MovieTitle = existingProjections.Movie.Title, 
                        ProjectionTime = existingProjections.DateTime
                    }
                };
                return errorModel;
            }
            foreach (var ticket in ticketsInProjection)
            {
                _ticketRepository.Delete(ticket.Id);
            }
            _projectionsRepository.Delete(id);

            _projectionsRepository.Save();

            ProjectionResultModel domainModel = new ProjectionResultModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Projection = new ProjectionDomainModel
                {
                    AuditoriumName = existingProjections.Auditorium.Name,
                    AuditoriumId = existingProjections.AuditoriumId,
                    Id = existingProjections.Id,
                    MovieId = existingProjections.MovieId,
                    MovieTitle = existingProjections.Movie.Title
                }
            };
            return domainModel;
        }

    }
}
