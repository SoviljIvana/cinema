using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class TicketPaymentConfirm
    {
        public List<CreateTicketModel> listOfTickets { get; set; }
    }
}
