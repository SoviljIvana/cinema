using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CreateCinemaDomainModel
    {
        public string CinemaName { get; set; }

        public List<AuditoriumDomainModel> listOfAuditoriums { get; set; }
    }
}
