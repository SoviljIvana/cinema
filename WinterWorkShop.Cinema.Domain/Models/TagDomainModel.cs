using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class TagDomainModel
    {
        public List<TagForMovieDomainModel> genres { get; set; }
        public List<TagForMovieDomainModel> actores { get; set; }
        public List<TagForMovieDomainModel> languages { get; set; }
        public List<TagForMovieDomainModel> states { get; set; }
        public List<TagForMovieDomainModel> awords { get; set; }
        public List<TagForMovieDomainModel> directors { get; set; }
    }
}
