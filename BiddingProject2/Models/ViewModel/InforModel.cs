using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models.ViewModel
{
    public class InforModel
    {
        [Required(ErrorMessage = "Name can not be blank")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Please input at least one character")]
        public string Name { get; set; }
        public string Occupation { get; set; }
        [Range(18, 90, ErrorMessage = "Your age is not suitable.")]
        public int Age { get; set; }
        public bool Gender { get; set; }
        [Required(ErrorMessage = "Email can not be blank")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
        public HttpPostedFileBase ProfileImageFile { get; set; }
    }
}