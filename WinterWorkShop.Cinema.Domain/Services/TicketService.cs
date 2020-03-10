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
        private readonly IUsersRepository _usersRepository;
        public TicketService(ITicketRepository ticketRepository, IUsersRepository usersRepository)
        {
            _ticketRepository = ticketRepository;
            _usersRepository = usersRepository;
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
            var userIdFind = _usersRepository.GetByUserName(ticketDomainModel.UserName);
            if (userIdFind==null)
            {
                return new CreateTicketResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.USER_NOT_FOUND
                };
            }
            Ticket newTicket = new Ticket
            {
                ProjectionId = ticketDomainModel.ProjectionId,
                SeatId = ticketDomainModel.SeatId,
                UserId = userIdFind.Id,
                Paid = false
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

        public async Task<PaymentResponse> ConfirmPayment(string username)
        {
            var userCheck = _usersRepository.GetByUserName(username);
            if (userCheck == null)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = Messages.USER_NOT_FOUND
                };
            }
            var tickets = await _ticketRepository.GetAllUnpaidForSpecificUser(username);
            if (tickets == null)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = Messages.TICKET_NOT_FOUND
                };
            }

            var newListOfTickets = new List<Ticket>();

            foreach (var ticket in tickets)
            {
                newListOfTickets.Add(new Ticket()
                {
                    Id = ticket.Id,
                    Paid = true,
                    ProjectionId = ticket.ProjectionId,
                    SeatId = ticket.SeatId,
                    UserId = ticket.UserId
                });
            }

            foreach (var ticketForUpdate in newListOfTickets)
            {
                var updatedTicket = _ticketRepository.Update(ticketForUpdate);
                if (updatedTicket == null)
                {
                    return new PaymentResponse
                    {
                        Message = Messages.TICKET_UPDATE_ERROR,
                        IsSuccess = false
                    };
                }
            }

            Data.User userWithPoints = new Data.User()
            {
                FirstName = userCheck.FirstName,
                BonusPoints = userCheck.BonusPoints + 1,
                Id = userCheck.Id,
                IsAdmin = userCheck.IsAdmin,
                LastName = userCheck.LastName,
                UserName = userCheck.UserName
            };

            var updateCheck = _usersRepository.Update(userWithPoints);
            if (updateCheck == null)
            {
                return new PaymentResponse
                {
                    Message = Messages.TICKET_UPDATE_ERROR,
                    IsSuccess = false
                };
            }
            _ticketRepository.Save();

            return new PaymentResponse
            {
                IsSuccess = true,
                Message = null
            };
        }

        public async Task<PaymentResponse> DeleteTicketsPaymentUnsuccessful(string username)
        {
            var userCheck = _usersRepository.GetByUserName(username);
            if (userCheck == null)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = Messages.USER_NOT_FOUND
                };
            }

            var tickets = await _ticketRepository.GetAllUnpaidForSpecificUser(username);
            if (tickets == null)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = Messages.TICKET_NOT_FOUND
                };
            }

            foreach (var ticket in tickets)
            {
                _ticketRepository.Delete(ticket.Id);
            }

            _ticketRepository.Save();

            return new PaymentResponse
            {
                IsSuccess = true,
                Message = Messages.PAYMENT_UNSUCCESSFUL
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

        public async Task<CreateTicketResultModel> DeleteTicketById(Guid id)
        {
            var data = _ticketRepository.Delete(id);

            if (data == null)
            {
                return null; 
            }

            _ticketRepository.Save();

            CreateTicketResultModel deletedTicket = new CreateTicketResultModel()
            {
                ErrorMessage = null, 
                IsSuccessful =  true, 
                Ticket = new TicketDomainModel
                {
                    Id = data.Id,
                    SeatId = data.SeatId,
                    UserId = data.UserId,
                    Paid = false
                }
            };
            return deletedTicket; 
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

        public async Task<IEnumerable<TicketDomainModel>> GetAllTicketsForThisUser(string username)
        {
            var data = await _ticketRepository.GetAllUnpaidForSpecificUser(username);
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
                    UserId = item.UserId,
                    ProjectionTime =  item.Projection.DateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss")
                };
                result.Add(model);
            }

            return result;
        }
    }
}
