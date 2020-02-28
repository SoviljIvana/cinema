using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("ticket")]
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid SeatId { get; set; }
        public Guid ProjectionId { get; set; }
        public Guid UserId { get; set; }
        public bool Paid { get; set; }
        public virtual Seat Seat { get; set; }
        public virtual Projection Projection { get; set; }
        public virtual  User User { get; set; }
    }
}
