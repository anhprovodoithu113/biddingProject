using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public virtual Product Product { get; set; }
        public virtual Category Category { get; set; }
    }
}