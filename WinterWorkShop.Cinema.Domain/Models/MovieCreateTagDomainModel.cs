using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class MovieCreateTagDomainModel
    {
        public List<string> tagsForMovieToAdd { get; set; }
        public int Duration { get; set; }
        
    }
}
