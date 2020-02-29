using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<TicketDomainModel>> GetAllTickets()
        {
            var data = await _ticketRepository.GetAll();
            if (data == null)
            {
                return null;
            }

            List<TicketDomainModel> result = new List<TicketDomainModel>();

            TicketDomainModel model;
            foreach (var item in data)
            {

                model = new TicketDomainModel
                {
                    Id = item.Id,
                    Paid = item.Paid,
                    SeatId = item.SeatId,
                    SeatNumber = item.Seat.Number,
                    SeatRow = item.Seat.Row,
                    MovieName = item.Projection.Movie.Title,
                    ProjectionId = item.ProjectionId,
                    AuditoriumName = item.Seat.Auditorium.Name,
                    CinemaName = item.Seat.Auditorium.Cinema.Name,
                    UserName = item.User.FirstName + " " + item.User.LastName,
                    UserId = item.UserId
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateTicketResultModel> CreateNewTicket(TicketDomainModel ticketDomainModel)
        {

            Ticket newTicket = new Ticket
            {
                ProjectionId = ticketDomainModel.ProjectionId,
                SeatId = ticketDomainModel.SeatId,
                UserId = ticketDomainModel.UserId
            };

            Ticket InsertedTicket = _ticketRepository.Insert(newTicket);

            if (InsertedTicket == null)
            {
                return new CreateTicketResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.TICKET_CREATION_ERROR
                };
            }

            _ticketRepository.Save();
            CreateTicketResultModel resultModel = new CreateTicketResultModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Ticket = new TicketDomainModel
                {
                    Id = InsertedTicket.Id,
                    ProjectionId = InsertedTicket.ProjectionId,
                    SeatId = InsertedTicket.SeatId,
                    UserId = InsertedTicket.UserId
                }
            };

            return resultModel;
        }

        public async Task<PaymentResponse> ConfirmPayment(List<TicketDomainModel> ticketDomainModels)
        {
            foreach (var item in ticketDomainModels)
            {
                var data = await _ticketRepository.GetByIdAsync(item.Id);

                if (data == null)
                {
                    return new PaymentResponse
                    {
                        IsSuccess = false,
                        Message = Messages.TICKET_NOT_FOUND
                    };
                }

                Ticket ticket = new Ticket()
                {
                    Id = data.Id,
                    Paid = true,
                    UserId = data.UserId,
                    ProjectionId = data.ProjectionId,
                    SeatId = data.SeatId,
                };

                var updatedTicket = _ticketRepository.Update(ticket);
                if (updatedTicket == null)
                {
                    return new PaymentResponse
                    {
                        Message = Messages.TICKET_UPDATE_ERROR,
                        IsSuccess = false
                    };
                }

                _ticketRepository.Save();

                PaymentResponse paymentResponse = new PaymentResponse()
                {
                    Message = null,
                    IsSuccess = true
                };

            }

            return new PaymentResponse
            {
                IsSuccess = false,
                Message = Messages.TICKET_NOT_FOUND
            };
        }

        public async Task<TicketDomainModel> DeleteTicket(Guid id) 
        {
            var ticketsForSeat = _ticketRepository.GetAllForSpecificSeat(id);

            if (ticketsForSeat == null) 
            {
                return null; 
            }

            foreach (var item in ticketsForSeat)
            {
                _ticketRepository.Delete(item.Id);
            }


            return null; 
        }

        public async Task<TicketDomainModel> DeleteTicketFromProjection(Guid id)
        {
            var ticketsForProjection = _ticketRepository.GetAllForSpecificProjection(id);

            if (ticketsForProjection == null)
            {
                return null;
            }

            foreach (var item in ticketsForProjection)
            {
                _ticketRepository.Delete(item.Id);
            }
            return null;
        }
    }
}
