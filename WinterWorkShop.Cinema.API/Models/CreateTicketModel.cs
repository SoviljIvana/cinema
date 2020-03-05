using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateTicketModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectionId { get; set; }
        public string UserName { get; set; }
        public List<SeatModel> seatModels { get; set; }
    }
}
