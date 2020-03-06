using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class SeatDomainModel
    {
        public Guid Id { get; set; }

        public int AuditoriumId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }
        public bool Reserved { get; set; }
        public bool Selected { get; set; }
        public int Counter { get; set; }
    }
}
