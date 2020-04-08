using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ITicketRepository _ticketRepository;

        public UserService(IUsersRepository usersRepository, ITicketRepository ticketRepository)
        {
            _usersRepository = usersRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<UserDomainModel>> GetAllAsync()
        {
            var data = await _usersRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<UserDomainModel> result = new List<UserDomainModel>();
            UserDomainModel model;
            foreach (var item in data)
            {
                model = new UserDomainModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserName = item.UserName,
                    IsAdmin = item.IsAdmin,
                    IsSuperUser = item.IsSuperUser,
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<UserDomainModel> GetUserByIdAsync(Guid id)
        {
            var data = await _usersRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                IsAdmin = data.IsAdmin,
                IsSuperUser = data.IsSuperUser
            };

            return domainModel;
        }

        public async Task<UserDomainModel> GetUserByUserName(string username)
        {
            var data = _usersRepository.GetByUserName(username);

            if (data == null)
            {
                return null;
            }

            var listOfTickets = _ticketRepository.GetAllForSpecificUser(data.Id);

            var ticketsList = new List<TicketDomainModel>();
            foreach (var item in listOfTickets)
            {
                ticketsList.Add(new TicketDomainModel 
                {
                    MovieName = item.Projection.Movie.Title,
                    AuditoriumName = item.Projection.Auditorium.Name,
                    CinemaName = item.Projection.Auditorium.Cinema.Name,
                    ProjectionTime = item.Projection.DateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                    SeatNumber = item.Seat.Number,
                    SeatRow = item.Seat.Row
                });
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                IsAdmin = data.IsAdmin,
                BonusPoints = data.BonusPoints,
                Tickets = ticketsList,
                IsSuperUser = data.IsSuperUser
                
            };

            return domainModel;
        }
    }
}
