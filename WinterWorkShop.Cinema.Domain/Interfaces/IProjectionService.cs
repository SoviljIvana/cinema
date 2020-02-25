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
        Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel);
        Task<IEnumerable<CreateProjectionFilterResultModel>> FilterAllProjections(string searchData);
        Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByMovieName(string searchData);
        Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByCinemaName(string searchData);
        Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByAuditoriumName(string searchData);
        Task<IEnumerable<CreateProjectionFilterResultModel>> FilterProjectionsByDates(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ProjectionDomainModel>> GetAllAsyncForSpecificMovie(Guid id);


        Task<CreateProjectionResultModel> DeleteProjection(Guid id);



    }
}