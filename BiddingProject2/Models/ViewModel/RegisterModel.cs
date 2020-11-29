using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models.ViewModel
{
    public class RegisterModel
    {
        [StringLength(20)]
        public string Username { get; set; }
        [StringLength(20)]
        public string Password { get; set; }
        public string RePassword { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}