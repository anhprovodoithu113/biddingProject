using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "The name of category can not be blank")]
        public string Name { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}