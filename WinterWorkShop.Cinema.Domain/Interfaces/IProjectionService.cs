using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IProjectionService
    {
        Task<IEnumerable<ProjectionDomainModel>> GetAllAsync();
        Task<ProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel);
        Task<CreateProjectionFilterResultModel> FilterAllProjections(string searchData);
        Task<CreateProjectionFilterResultModel> FilterProjectionsByMovieName(string searchData);
        Task<CreateProjectionFilterResultModel> FilterProjectionsByCinemaName(string searchData);
        Task<CreateProjectionFilterResultModel> FilterProjectionsByAuditoriumName(string searchData);
        Task<CreateProjectionFilterResultModel> FilterProjectionsByDates(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ProjectionDomainModel>> GetAllAsyncForSpecificMovie(Guid id);


        Task<ProjectionResultModel> DeleteProjection(Guid id);



    }
}