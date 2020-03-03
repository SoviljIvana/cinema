using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class TagDomainModel
    {
        public List<string> genres { get; set; }
        public List<string> actores { get; set; }
        public List<string> creators { get; set; }
        public List<string> languages { get; set; }
        public List<string> states { get; set; }
        public List<string> awords { get; set; }
    }
}
