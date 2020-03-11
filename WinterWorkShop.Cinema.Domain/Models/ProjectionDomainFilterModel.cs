using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ProjectionDomainFilterModel
    {
        public Guid Id { get; set; }
        public string MovieTitle { get; set; }
        public string CinemaName { get; set; }
        public string AuditoriumName { get; set; }
        public DateTime ProjectionTime { get; set; }


    }
}
