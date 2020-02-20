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

        public async Task<IEnumerable<ProjectionDomainFilterModel>> FilterAllProjections(string searchData)
        {
            var data = _projectionsRepository.FilterAllProjections(searchData).Result.ToList();
            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainFilterModel> result = new List<ProjectionDomainFilterModel>();
            ProjectionDomainFilterModel model; 

            foreach(var item in data)
            {
                model = new ProjectionDomainFilterModel
                {
                    AditoriumName = item.Auditorium.Name, 
                    MovieTitle = item.Movie.Title, 
                    ProjectionTime = item.DateTime, 
                    CinemaName = item.Auditorium.Cinema.Name 
                };
                result.Add(model); 
            }
            return result; 
        }

        public async Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectionsByMovieName(string searchData)
        {
            var data = _projectionsRepository.FilterProjectionsByMovieTitle(searchData).Result.ToList();
            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainFilterModel> result = new List<ProjectionDomainFilterModel>();
            ProjectionDomainFilterModel model;

            foreach (var item in data)
            {
                model = new ProjectionDomainFilterModel
                {
                    AditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                result.Add(model);
            }
            return result;
        }

        public async Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectionsByCinemaName(string searchData)
        {
            var data = _projectionsRepository.FilterProjectionsByCinemaName(searchData).Result.ToList();
            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainFilterModel> result = new List<ProjectionDomainFilterModel>();
            ProjectionDomainFilterModel model;

            foreach (var item in data)
            {
                model = new ProjectionDomainFilterModel
                {
                    AditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                result.Add(model);
            }
            return result;
        }

        public async Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectiondByAuditoriumName(string searchData)
        {
            var data = _projectionsRepository.FilterProjectionsByAuditoriumName(searchData).Result.ToList();
            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainFilterModel> result = new List<ProjectionDomainFilterModel>();
            ProjectionDomainFilterModel model;

            foreach (var item in data)
            {
                model = new ProjectionDomainFilterModel
                {
                    AditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                result.Add(model);
            }
            return result;
        }

        public async Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectiondByDates(DateTime startDate, DateTime endDate)
        {
            var data = _projectionsRepository.FilterProjectionsByDates(startDate, endDate).Result.ToList();
            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainFilterModel> result = new List<ProjectionDomainFilterModel>();
            ProjectionDomainFilterModel model;

            foreach (var item in data)
            {
                model = new ProjectionDomainFilterModel
                {
                    AditoriumName = item.Auditorium.Name,
                    MovieTitle = item.Movie.Title,
                    ProjectionTime = item.DateTime,
                    CinemaName = item.Auditorium.Cinema.Name
                };
                result.Add(model);
            }
            return result;
        }
    }
}
