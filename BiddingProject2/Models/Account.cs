using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<ChitChat> ChitChats { get; set; }
    }
}