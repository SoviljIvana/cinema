using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemasRepository _cinemasRepository;

        public CinemaService(ICinemasRepository cinemasRepository)
        {
            _cinemasRepository = cinemasRepository;
        }

        public async Task<IEnumerable<CinemaDomainModel>> GetAllAsync()
        {
            var data = await _cinemasRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<CinemaDomainModel> result = new List<CinemaDomainModel>();
            CinemaDomainModel model;
            foreach (var item in data)
            {
                model = new CinemaDomainModel
                {
                    Id = item.Id,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

       public async Task<CinemaDomainModel> AddCinema(CinemaDomainModel newCinema)
        {
            Data.Cinema cinemaToCreate = new Data.Cinema()
            {
                Name = newCinema.Name
            };

            var data = _cinemasRepository.Insert(cinemaToCreate);
            if (data == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }



        public Task<CinemaDomainModel> GetCinemaByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

      

        public Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateMovie)
        {
            throw new NotImplementedException();
        }

        public Task<CinemaDomainModel> DeleteCinema(Guid id)
        {
            throw new NotImplementedException();
        }
    }

}
