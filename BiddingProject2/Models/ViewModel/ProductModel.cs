using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models.ViewModel
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
    }
}