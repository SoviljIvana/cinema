using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;

        public ProjectionService(IProjectionsRepository projectionsRepository)
        {
            _projectionsRepository = projectionsRepository;
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
                    AditoriumName = item.Auditorium.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.DateTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.DateTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new CreateProjectionResultModel
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
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            CreateProjectionResultModel result = new CreateProjectionResultModel
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

        public async Task<IEnumerable<CreateProjectionFilterResultModel>> FilterAllProjections(string searchData)
        {
            var data = await _projectionsRepository.FilterAllProjections(searchData);

            List<CreateProjectionFilterResultModel> result = new List<CreateProjectionFilterResultModel>();

            if (data == null)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                });
                return result;
            }
            var n = data.Count();


            if (data.Count() == 0)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = "Auditorium not found",
                        MovieTitle = "Movie not found.",
                        CinemaName = "Cinema not found",
                    }
                });
                return result;
            }

            foreach (var item in data)
            {
                CreateProjectionFilterResultModel model = new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_SUCCESSFUL,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                    }
                };
                result.Add(model);
            }
            return result;
        }

        public async Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByMovieName(string searchData)
        {
            var data = await _projectionsRepository.FilterProjectionsByMovieTitle(searchData);

            List<CreateProjectionFilterResultModel> result = new List<CreateProjectionFilterResultModel>();

            if (data == null)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                });
                return result;
            }
            var n = data.Count();


            if (data.Count() == 0)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = "Auditorium not found",
                        MovieTitle = "Movie not found.",
                        CinemaName = "Cinema not found",
                    }
                });
                return result;
            }

            foreach (var item in data)
            {
                CreateProjectionFilterResultModel model = new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_SUCCESSFUL,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                    }
                };
                result.Add(model);
            }
            return result;

        }

        public async Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByCinemaName(string searchData)
        {
            var data = await _projectionsRepository.FilterProjectionsByCinemaName(searchData);

            List<CreateProjectionFilterResultModel> result = new List<CreateProjectionFilterResultModel>();

            if (data == null)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                });
                return result;
            }
            var n = data.Count();


            if (data.Count() == 0)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projection = new ProjectionDomainFilterModel
                    {
                        CinemaName = "Cinema not found",
                        AditoriumName = "Auditorium not found",
                        MovieTitle = "Movie not found.",
                    }
                });
                return result;
            }

            foreach (var item in data)
            {
                CreateProjectionFilterResultModel model = new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_SUCCESSFUL,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                    }
                };
                result.Add(model);
            }
            return result;

        }

        public async Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByAuditoriumName(string searchData)
        {
            var data = await _projectionsRepository.FilterProjectionsByAuditoriumName(searchData);

            List<CreateProjectionFilterResultModel> result = new List<CreateProjectionFilterResultModel>();

            if (data == null)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                });
                return result;
            }
            var n = data.Count();


            if (data.Count() == 0)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projection = new ProjectionDomainFilterModel
                    {
                        CinemaName = "Cinema not found",
                        AditoriumName = "Auditorium not found",
                        MovieTitle = "Movie not found.",
                    }
                });
                return result;
            }

            List<ProjectionDomainFilterModel> listProjDomMode = new List<ProjectionDomainFilterModel>();

            foreach (var item in data)
            {
                CreateProjectionFilterResultModel model = new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_SUCCESSFUL,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                    }
                };
                result.Add(model);
            }
            return result;
        }




        public async Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByDates(DateTime startDate, DateTime endDate)
        {
            var data = await _projectionsRepository.FilterProjectionsByDates(startDate, endDate);

            List<CreateProjectionFilterResultModel> result = new List<CreateProjectionFilterResultModel>();

            if (data == null)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_SEARCH_ERROR
                });
                return result;
            }
            var n = data.Count();


            if (data.Count() == 0)
            {
                result.Add(new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_NORESULT,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = "Auditorium not found",
                        MovieTitle = "Movie not found.",
                        CinemaName = "Cinema not found",
                    }
                });
                return result;
            }

            foreach (var item in data)
            {
                CreateProjectionFilterResultModel model = new CreateProjectionFilterResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = Messages.PROJECTION_SEARCH_SUCCESSFUL,
                    Projection = new ProjectionDomainFilterModel
                    {
                        AditoriumName = item.Auditorium.Name,
                        MovieTitle = item.Movie.Title,
                        ProjectionTime = item.DateTime,
                        CinemaName = item.Auditorium.Cinema.Name
                    }
                };
                result.Add(model);
            }
            return result;
        }

        public async Task<CreateProjectionResultModel> DeleteProjection(Guid id)
        {

            var existingProjections = _projectionsRepository.GetProjectionById(id);

            if (existingProjections == null)
            {
                CreateProjectionResultModel errorModel = new CreateProjectionResultModel
                {
                    ErrorMessage = Messages.PROJECTION_IN_FUTURE,
                    IsSuccessful = true
                };
                return errorModel;
            }

            if (existingProjections.DateTime > DateTime.Now)
            {
                CreateProjectionResultModel errorModel = new CreateProjectionResultModel
                {
                    ErrorMessage = Messages.PROJECTION_IN_FUTURE,
                    IsSuccessful = false,
                    Projection = new ProjectionDomainModel
                    {
                        AuditoriumId = existingProjections.AuditoriumId,
                        Id = existingProjections.Id,
                        MovieId = existingProjections.MovieId,
                        AditoriumName = existingProjections.Auditorium.Name, 
                        MovieTitle = existingProjections.Movie.Title, 
                        ProjectionTime = existingProjections.DateTime
                    }
                };
                return errorModel;
            }

            _projectionsRepository.Delete(id);

            _projectionsRepository.Save();

            CreateProjectionResultModel domainModel = new CreateProjectionResultModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Projection = new ProjectionDomainModel
                {
                    AditoriumName = existingProjections.Auditorium.Name,
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
