using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class ChitChat
    {
        public int ChitChatId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public virtual Account Account { get; set; }
        public virtual Product Product { get; set; }
    }
}