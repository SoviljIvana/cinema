﻿using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class MovieDomainModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Current { get; set; }
        public double Rating { get; set; }
        public int Year { get; set; }
        public List<ProjectionDomainModel> listOfProjections { get; set; }
    }
}
