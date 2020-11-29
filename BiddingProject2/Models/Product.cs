using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage ="Name of product can not be blank")]
        public string Name { get; set; }
        public string Image { get; set; }
        public float BeginPrice { get; set; }
        public float EndPrice { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        public bool State { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public HttpPostedFileBase ProductImageFile { get; set; }
        public virtual ICollection<ProductCategory> Categories { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<ChitChat> ChitChats { get; set; }
        public virtual Order Order { get; set; }

    }
}