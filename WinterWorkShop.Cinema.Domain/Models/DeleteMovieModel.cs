using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class DeleteMovieModel
    {
        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }
        public MovieDomainModel MovieDomainModel { get; set; }
    }
}
