using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Order
    {
        [ForeignKey("Product")]
        public int OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public float Price { get; set; }

        // Đây là mã đơn hàng
        public string Number { get; set; }

        // Kiểm tra xem đơn hàng được giao hay chưa
        // 0. Processing
        // 1. Transfering
        // 2. Transfer Successful
        // 3. Canceled
        public int State { get; set; }
        public virtual Account Account { get; set; }

        public virtual Product Product { get; set; }
    }
}