using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class TicketPaymentConfirm
    {
        public string UserName { get; set; }
        public bool PaymentSuccess { get; set; }
    }
}
