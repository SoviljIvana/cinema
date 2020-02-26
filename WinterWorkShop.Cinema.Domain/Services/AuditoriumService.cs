﻿using System;
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
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly ICinemasRepository _cinemasRepository;
        private readonly ISeatsRepository _seatsRepository;
        private readonly IProjectionService _projectionService;
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly ISeatService _seatService; 



        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository, 
                                ICinemasRepository cinemasRepository, 
                                ISeatsRepository seatsRepository,
                                IProjectionService projectionService,
                                IProjectionsRepository projectionsRepository, 
                                ISeatService seatService)
        {
            _auditoriumsRepository = auditoriumsRepository;
            _cinemasRepository = cinemasRepository;
            _seatsRepository = seatsRepository;
            _projectionService = projectionService;
            _projectionsRepository = projectionsRepository;
            _seatService = seatService;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> GetAllAsync()
        {
            var data = await _auditoriumsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<AuditoriumDomainModel> result = new List<AuditoriumDomainModel>();
            AuditoriumDomainModel model;
            foreach (var item in data)
            {
                model = new AuditoriumDomainModel
                {
                    Id = item.Id,
                    CinemaId = item.CinemaId,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateAuditoriumResultModel> CreateAuditorium(AuditoriumDomainModel domainModel, int numberOfRows, int numberOfSeats)
        {
            var cinema = await _cinemasRepository.GetByIdAsync(domainModel.CinemaId);
            if (cinema == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_UNVALID_CINEMAID
                };
            }

            var auditorium = await _auditoriumsRepository.GetByAuditName(domainModel.Name, domainModel.CinemaId);
            var sameAuditoriumName = auditorium.ToList();
            if (sameAuditoriumName != null && sameAuditoriumName.Count > 0)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_SAME_NAME
                };
            }

            Auditorium newAuditorium = new Auditorium
            {
                Name = domainModel.Name,
                CinemaId = domainModel.CinemaId,
            };

            newAuditorium.Seats = new List<Seat>();

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat newSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    newAuditorium.Seats.Add(newSeat);
                }
            }

            Auditorium insertedAuditorium = _auditoriumsRepository.Insert(newAuditorium);

            if (insertedAuditorium == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR
                };
            }
            _auditoriumsRepository.Save();
            CreateAuditoriumResultModel resultModel = new CreateAuditoriumResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = new AuditoriumDomainModel
                {
                    Id = insertedAuditorium.Id,
                    Name = insertedAuditorium.Name,
                    CinemaId = insertedAuditorium.CinemaId,
                    SeatsList = new List<SeatDomainModel>()
                }
            };

            foreach (var item in insertedAuditorium.Seats)
            {
                resultModel.Auditorium.SeatsList.Add(new SeatDomainModel
                {
                    AuditoriumId = insertedAuditorium.Id,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                });
            }

            return resultModel;
        }

        public async Task<AuditoriumDomainModel> GetAuditoriumByIdAsync(int id)
        {
            var data = await _auditoriumsRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }
            AuditoriumDomainModel domainModel = new AuditoriumDomainModel
            {
                Id = data.Id,
                Name = data.Name,
                CinemaId = data.CinemaId
            };

            return domainModel;
        }
  
        public async Task<AuditoriumDomainModel> UpdateAuditorium(AuditoriumDomainModel auditoriumToUpdate)
        {

            Auditorium auditorium = new Auditorium()
            {
                Id = auditoriumToUpdate.Id,
                Name = auditoriumToUpdate.Name,
                CinemaId = auditoriumToUpdate.CinemaId
                
            };

            var data = _auditoriumsRepository.Update(auditorium);

            if (data == null)
            {
                return null;
            }
            _auditoriumsRepository.Save();

            AuditoriumDomainModel domainModel = new AuditoriumDomainModel()
            {
                Id = data.Id,
                Name = data.Name,
                CinemaId = data.CinemaId

            };

            return domainModel;

        }

        public async Task<CreateAuditoriumResultModel> DeleteAuditorium(int id)
        {
            var existingAuditorium = await _auditoriumsRepository.GetByIdAsync(id);
            var projectionsInAuditorium = _projectionsRepository.GetAllOfSpecificAuditorium(id);
            var seatsInAuditorium = _seatsRepository.GetAllOfSpecificAuditorium(id);

            if (existingAuditorium == null)
            {
                CreateAuditoriumResultModel errorModel = new CreateAuditoriumResultModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    IsSuccessful = false
                };
                return errorModel;
            }

            existingAuditorium.Projections = projectionsInAuditorium.ToList();
            existingAuditorium.Seats = seatsInAuditorium.ToList();

            if (existingAuditorium != null)
            {
                foreach (var projection in projectionsInAuditorium)
                {
                    if (projection.DateTime > DateTime.Now)
                    {
                        {
                            CreateAuditoriumResultModel errorModel = new CreateAuditoriumResultModel
                            {
                                ErrorMessage = Messages.PROJECTION_IN_FUTURE,
                                IsSuccessful = false,
                                Auditorium = new AuditoriumDomainModel
                                {
                                    CinemaId = existingAuditorium.CinemaId,
                                    Id = existingAuditorium.Id,
                                    Name = existingAuditorium.Name,
                                    NumberOfSeats = existingAuditorium.Seats.Select(x => x.Number).Count(),
                                    SeatRows = existingAuditorium.Seats.Select(x => x.Row).Count()
                                }
                            };
                            return errorModel;
                        }
                    }
                    _projectionsRepository.Delete(projection.Id);
                }
                foreach (var seat in seatsInAuditorium)
                {
                    _seatsRepository.Delete(seat.Id);
                }
            }

            var data = _auditoriumsRepository.Delete(id);
            _auditoriumsRepository.Save();

            CreateAuditoriumResultModel domainModel = new CreateAuditoriumResultModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Auditorium = new AuditoriumDomainModel
                {
                    CinemaId = existingAuditorium.CinemaId,
                    Id = existingAuditorium.Id,
                    Name = existingAuditorium.Name,
                    NumberOfSeats = existingAuditorium.Seats.Select(x => x.Number).Count(),
                    SeatRows = existingAuditorium.Seats.Select(x => x.Row).Count(),
                }
            };

            return domainModel;
        }
        public IEnumerable<AuditoriumDomainModel> GetAllOfSpecificCinema(int id)
        {
            var data = _auditoriumsRepository.GetAllOfSpecificCinema(id);

            if (data == null)
            {
                return null;
            }

            List<AuditoriumDomainModel> result = new List<AuditoriumDomainModel>();
            AuditoriumDomainModel model;
            foreach (var item in data)
            {
                model = new AuditoriumDomainModel
                {
                    Id = item.Id,
                    CinemaId = item.CinemaId,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

    }
}
