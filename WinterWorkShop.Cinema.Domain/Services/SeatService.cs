using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatsRepository _seatsRepository;
        private readonly ITicketRepository _ticketRepository;

        public SeatService(ISeatsRepository seatsRepository, ITicketRepository ticketRepository)
        {
            _seatsRepository = seatsRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<SeatDomainModel>> GetAllAsync()
        {
            var data = await _seatsRepository.GetAll();

            List<SeatDomainModel> result = new List<SeatDomainModel>();
            SeatDomainModel model;
            foreach (var item in data)
            {
                model = new SeatDomainModel
                {
                    Id = item.Id,
                    AuditoriumId = item.AuditoriumId,
                    Number = item.Number,
                    Row = item.Row
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<IEnumerable<SeatDomainModel>> GetAllSeatsForProjection(Guid id)
        {
            var allSeatsForProjection = await _seatsRepository.GetAllOfSpecificProjection(id);

            var reservedSeatsForThisProjection = _ticketRepository.GetAllForSpecificProjection(id);
            

            if (allSeatsForProjection == null)
            {
                return null;
            }

            List<SeatDomainModel> result = new List<SeatDomainModel>();
            SeatDomainModel model;

            foreach (var seat in allSeatsForProjection)
            {
                
                model = new SeatDomainModel
                {
                    AuditoriumId = seat.AuditoriumId,
                    Id = seat.Id,
                    Number = seat.Number,
                    Row = seat.Row
                };

                foreach (var reservedSeat in reservedSeatsForThisProjection)
                {
                    if (seat.Id == reservedSeat.SeatId)
                    {
                        model.Reserved = true;
                    }
                }
                result.Add(model);
            }

            return result;
        }


        public async Task<SeatDomainModel> DeleteSeat(Guid id)
        {
            var existingSeats = await _seatsRepository.GetByIdAsync(id);

            if (existingSeats == null)
            {
                return null;
            }

            _seatsRepository.Delete(id);

            _seatsRepository.Save();

            if (existingSeats.Tickets != null)
            {
                return null;
            }

            SeatDomainModel domainModel = new SeatDomainModel
            {
                AuditoriumId = existingSeats.AuditoriumId, 
                Id = existingSeats.Id, 
                Number = existingSeats.Number, 
                Row = existingSeats.Row
            };

            return domainModel;

        }
    }
}
