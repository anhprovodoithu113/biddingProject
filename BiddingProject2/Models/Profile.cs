using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Profile
    {
        [ForeignKey("Account")]
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Name can not be blank")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Please input at least one character")]
        public string Name { get; set; }
        public string Occupation { get; set; }
        [Range(18, 90, ErrorMessage = "Your age is not suitable")]
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}