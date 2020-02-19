using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("tag")]
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Description> Descriptions { get; set; }


    }
}
