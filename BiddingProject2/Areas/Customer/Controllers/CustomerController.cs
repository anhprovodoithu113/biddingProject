using BiddingProject2.DAL;
using BiddingProject2.Models;
using BiddingProject2.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiddingProject2.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        BiddingDbContext db = new BiddingDbContext();
        Random rand = new Random();
        // GET: Customer/Customer

        // Private Information
        public ActionResult Index(string msg = "")
        {
            string username = Session["userLogin"].ToString();
            Account acc = db.Accounts.SingleOrDefault(m => m.Username == username);
            Profile prof = db.Profiles.FirstOrDefault(m => m.ProfileId == acc.AccountId);
            ViewBag.Message = msg;
            return View(prof);
        }

        [HttpPost]
        public ActionResult Index(InforModel infor)
        {
            string msg = "";
            Profile prof = db.Profiles.FirstOrDefault(m => m.Account.Username == infor.Username);
            if (prof == null)
            {
                return HttpNotFound();
            }
            try
            {
                if (infor.ProfileImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(infor.ProfileImageFile.FileName);
                    string extension = Path.GetExtension(infor.ProfileImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;

                    prof.Image = "~/Images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Images/" + fileName));
                    infor.ProfileImageFile.SaveAs(fileName);
                }
                prof.Name = infor.Name;
                prof.Gender = infor.Gender;
                prof.Age = infor.Age;
                prof.Occupation = infor.Occupation;
                msg = "Change information successful!";
                db.SaveChanges();
            }
            catch (Exception e)
            {
                msg = "Cannot save. Please try again!";
            }
            return RedirectToAction("Index", new { msg = msg });
        }

        // Bidding Cart
        public ActionResult BidCart(string msg = "")
        {
            string username = Session["userLogin"].ToString();
            Account acc = db.Accounts.FirstOrDefault(m => m.Username == username);
            List<Result> lstResult = db.Results.Where(m => m.AccountId == acc.AccountId && m.State && !m.IsPurchase).ToList();
            ViewBag.Message = msg;
            return View(lstResult);
        }

        [HttpGet]
        public ActionResult DeleteItem(int idItem)
        {
            string msg = "";
            Result res = db.Results.Find(idItem);
            if(res != null)
            {
                try
                {
                    res.State = false;
                    db.SaveChanges();
                    msg = "Delete item in bidding cart successful!";
                }
                catch (Exception e)
                {
                    msg = "Delete failed. Please try again!";
                }
            }
            return RedirectToAction("BidCart", new { msg = msg });
        }

        public ActionResult PackageOrder()
        {
            string username = Session["userLogin"].ToString();
            Account acc = db.Accounts.FirstOrDefault(m => m.Username == username);
            List<Result> lstResult = db.Results.Where(m => m.AccountId == acc.AccountId && m.State && !m.IsPurchase).ToList();
            string[] lstNumber = new string[lstResult.Count];
            int i = 0;
            foreach (var item in lstResult)
            {
                int orderId = rand.Next(10000, 99999);
                int productId = item.ProductId;
                float price = item.Price;
                string number = "M" + orderId;
                Product prod = db.Products.Find(item.ProductId);
                Order order = new Order() { Number = number, State = 0, CreatedDate = DateTime.Now, Price = price, Product = prod, Account = acc };
                lstNumber[i++] = number;
                Result result = db.Results.Find(item.ResultId);
                result.IsPurchase = true;
                db.Orders.Add(order);
                db.SaveChanges();
            }
            return View(lstNumber);
        }

        public ActionResult ManageOrder()
        {
            List<Order> lstOrder = db.Orders.ToList();
            return View(lstOrder);
        }
    }
}