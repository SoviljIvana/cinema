using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateCinemaModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name{ get; set; }
    }
}
