using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class SeatModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
    }
}
