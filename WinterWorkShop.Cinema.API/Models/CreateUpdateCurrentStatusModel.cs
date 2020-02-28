using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateUpdateCurrentStatusModel
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public bool Current { get; set; }

    }
}
