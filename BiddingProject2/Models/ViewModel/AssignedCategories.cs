using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models.ViewModel
{
    public class AssignedCategories
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsAssigned { get; set; }
    }
}