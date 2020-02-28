using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateCinemaWithAuditoriumAndSeatsModel
    {
        public string CinemaName { get; set; }
        
        public List<CreateAuditoriumModel> listOfAuditoriums { get; set; }
}
}
