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
        Task<IEnumerable<ProjectionDomainFilterModel>> FilterAllProjections(string searchData);
        Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectionsByMovieName(string searchData);
        Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectionsByCinemaName(string searchData);
        Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectiondByAuditoriumName(string searchData);
        Task<IEnumerable<ProjectionDomainFilterModel>> FilterProjectiondByDates(DateTime startDate, DateTime endDate);





    }
}
