using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models.ViewModel
{
    public class CustomerModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int TimeToBidding { get; set; }
    }
}