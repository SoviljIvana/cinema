using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
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

        

        public async Task<CreateCinemaResultModel> AddCinema(CinemaDomainModel newCinema)
        {
            Data.Cinema cinemaToCreate = new Data.Cinema()
            {
                Name = newCinema.Name
            };

            var data = _cinemasRepository.Insert(cinemaToCreate);
            if (data == null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR
                };
            }

            _cinemasRepository.Save();

            CreateCinemaResultModel createCinemaResultModel = new CreateCinemaResultModel()
            {
                Cinema = new CinemaDomainModel 
                {
                    Id = data.Id,
                    Name = data.Name
                },
                IsSuccessful = true,
                ErrorMessage = null

            };

            return createCinemaResultModel;
        }

        public async Task<CreateCinemaResultModel> AddCinemaWithAuditoriumsAndSeats(CreateCinemaDomainModel newCinema)
        {
            var checkName = await _cinemasRepository.GetByNameAsync(newCinema.CinemaName);
            if (checkName == null)
            {
                return new CreateCinemaResultModel
                {
                    ErrorMessage = Messages.CINEMA_SAME_NAME,
                    IsSuccessful = false
                };
            }

            Data.Cinema newCinemaToAdd = new Data.Cinema
            {
                Name = newCinema.CinemaName,
                Auditoriums = new List<Auditorium>()
            };

            var listofAuditoriums = newCinema.listOfAuditoriums;

            foreach (var item in listofAuditoriums)
            {
                newCinemaToAdd.Auditoriums.Add(new Auditorium 
                {
                    Name = item.auditName,
                    Seats = new List<Seat>()
                });

                foreach (var auditorium in newCinemaToAdd.Auditoriums)
                {
                    for (int i = 1; i < item.SeatRows + 1; i++)
                    {
                        for (int j = 1; j < item.NumberOfSeats + 1; j++)
                        {
                            Seat seat = new Seat()
                            {
                                Row = i,
                                Number = j
                            };
                            auditorium.Seats.Add(seat);
                        }
                    }
                }
            }

            Data.Cinema insertedCinema = _cinemasRepository.Insert(newCinemaToAdd);
            if (insertedCinema == null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR
                };
            }

            _cinemasRepository.Save();

            CreateCinemaResultModel createCinemaResultModel = new CreateCinemaResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Cinema = new CinemaDomainModel
                {
                    Id = insertedCinema.Id,
                    Name = insertedCinema.Name,
                    AuditoriumDomainModels = new List<AuditoriumDomainModel>()
                }
            };

            foreach (var insertedAuditorium in insertedCinema.Auditoriums)
            {
                createCinemaResultModel.Cinema.AuditoriumDomainModels.Add(new AuditoriumDomainModel
                {
                    CinemaId = insertedAuditorium.CinemaId,
                    Id = insertedAuditorium.Id,
                    auditName = insertedAuditorium.Name,
                    SeatsList = new List<SeatDomainModel>()
                });

                var seatsForThisAuditorium = insertedAuditorium.Seats.ToList();

                foreach (var item in createCinemaResultModel.Cinema.AuditoriumDomainModels)
                {
                    foreach (var seat in seatsForThisAuditorium)
                    {
                        item.SeatsList.Add(new SeatDomainModel
                        {
                            Number = seat.Number,
                            Id = seat.Id,
                            Row = seat.Row,
                            AuditoriumId = seat.AuditoriumId
                        });
                    }
                }
            }
            return createCinemaResultModel;
        }


        public async Task<CinemaDomainModel> GetCinemaByIdAsync(int id)
        {
            var data = await _cinemasRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Id = data.Id,
                Name = data.Name

            };

            return domainModel;
        }


        public async Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateCinema)
        {

            Data.Cinema cinema = new Data.Cinema()
            {
                Id = updateCinema.Id,
                Name = updateCinema.Name

            };

            var data = _cinemasRepository.Update(cinema);

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

        public async Task<CinemaDomainModel> DeleteCinema(int id)
        {
            var data = _cinemasRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Name = data.Name,
                Id = data.Id
            };

            return domainModel;
        }
    }
}
