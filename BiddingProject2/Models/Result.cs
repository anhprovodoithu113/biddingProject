using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingProject2.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public DateTime CreatedDate { get; set; }

        // Đấu giá thành công hay chưa
        public bool State { get; set; }

        // Giá được đưa ra bởi người dùng
        public float Price { get; set; }

        // Đã trả tiền cho sản phẩm này hay chưa
        public bool IsPurchase { get; set; }

        public int ProductId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Product Product { get; set; }
    }
}