using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("description")]
    public class Description
    {
        public Guid DescriptionId { get; set; }
        public Guid MovieId { get; set; }
        public int TagId { get; set; }

    }
}
