﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}