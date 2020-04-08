using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class TicketDomainModel
    {
        public Guid Id { get; set; }
        public Guid SeatId { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public Guid ProjectionId { get; set; }
        public string MovieName { get; set; }
        public string CinemaName { get; set; }
        public string AuditoriumName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool Paid { get; set; }
        public string ProjectionTime { get; set; }

        //public List<SeatDomainModel> ReservedSeats { get; set; }
    }
}
