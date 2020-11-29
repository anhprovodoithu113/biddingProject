using BiddingProject2.DAL;
using BiddingProject2.Models;
using BiddingProject2.Models.ViewModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiddingProject2.Controllers
{
    public class HomeController : Controller
    {
        BiddingDbContext db = new BiddingDbContext();
        [AllowAnonymous]
        public ActionResult Index(int idCategory = 0, string msg = "All")
        {
            List<ProductModel> lstModel = new List<ProductModel>();
            if(idCategory == 0)
            {
                var groupBy = db.ProductCategories.Where(m => !m.Product.IsDeleted).GroupBy(m => m.Product).OrderByDescending(m => m.Key.ProductId).ToList();

                foreach (var item in groupBy)
                {
                    lstModel.Add(new ProductModel() { 
                        ProductId = item.Key.ProductId,
                        Image = item.Key.Image,
                        ProductName = item.Key.Name,
                        Price = item.Key.EndPrice,
                        Description = item.Key.Description,
                        StartDate = item.Key.StartDate
                    });
                }
            }
            else
            {
                var prodLst = db.ProductCategories
                    .Where(m => !m.Product.IsDeleted && m.Category.CategoryId == idCategory)
                    .Distinct().OrderByDescending(m => m.Product.ProductId).ToList();

                foreach (var item in prodLst)
                {
                    lstModel.Add(new ProductModel()
                    {
                        ProductId = item.Product.ProductId,
                        Image = item.Product.Image,
                        ProductName = item.Product.Name,
                        Price = item.Product.EndPrice,
                        Description = item.Product.Description,
                        StartDate = item.Product.StartDate
                    });
                }

                msg = db.Categories.FirstOrDefault(m => m.CategoryId == idCategory).Name;
            }
            if(Session["userLogin"] != null)
            {
                string username = Session["userLogin"].ToString();
                Account acc = db.Accounts.FirstOrDefault(m => m.Username == username);
                List<Result> lstResult = db.Results.Where(m => m.AccountId == acc.AccountId).ToList();
                DateTime current = DateTime.Now;
                foreach (var item in lstResult)
                {
                    TimeSpan checkDate = current.Subtract(item.CreatedDate);
                    if(checkDate.Days > 30)
                    {
                        item.State = false;
                        db.SaveChanges();
                    }
                }
            }
            
            
            ViewBag.IsLoggin = Session["userLogin"] == null ? false : true;
            ViewBag.ListCategory = db.Categories.Where(m => !m.IsDelete).ToList();
            ViewBag.DisplaySelect = msg;
            return View(lstModel);
        }

        [HttpGet]
        public ActionResult ProductDetail(int idProduct)
        {
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("Index", "Login", new { msg = "Please login before join in the bidding!"});
            }
            Product prod = db.Products.Find(idProduct);
            int a = db.Results.Where(m => m.ProductId == prod.ProductId).Count();
            if (a > 0)
            {
                List<Result> lstResult = db.Results.Where(m => m.ProductId == prod.ProductId).ToList();
                Result currentTop = lstResult.ElementAt(lstResult.Count - 1);
                ViewBag.CustomerName = db.Accounts.Find(currentTop.AccountId).Profile.Name;
                ViewBag.Price = currentTop.Price;
            }

            List<ChitChat> lstChat = db.ChitChats.Where(m => m.Product.ProductId == idProduct).ToList();
            if (lstChat.Count > 0)
            {
                ViewBag.ListChat = lstChat;
            }

            return View(prod);
        }

        [Authorize(Roles = "Customer")]

        [HttpGet]
        public JsonResult GetInfo()
        {
            if(Session["userLogin"] != null)
            {
                string username = Session["userLogin"].ToString();
                int countItem = db.Results.Count(m => m.State && m.Account.Username == username);
                var profile = from acc in db.Accounts
                              where acc.Username == username
                              select new
                              {
                                  img = acc.Profile.Image,
                                  name = acc.Profile.Name,
                                  total = countItem
                              };
                return Json(new { data = profile }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpGet]
        public JsonResult AddPrice(int idProduct, float price)
        {
            string username = Session["userLogin"].ToString();
            int accId = db.Accounts.FirstOrDefault(m => m.Username == username).AccountId;
            Result result = new Result()
            {
                AccountId = accId,
                ProductId = idProduct,
                CreatedDate = DateTime.Now,
                Price = price,
                State = false
            };
            db.Results.Add(result);
            db.SaveChanges();

            Product prod = db.Products.FirstOrDefault(m => m.ProductId == idProduct);
            try
            {
                prod.EndPrice = price;
                prod.State = true;
                db.SaveChanges();
                return Json(new { customerName = username, customerPrice = price }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AddProductToCart(int idProduct, float price)
        {
            Result res = db.Results.FirstOrDefault(m => m.ProductId == idProduct && m.Price == price);
            string username = "";
            if (res != null && !res.State)
            {
                try
                {
                    res.State = true;
                    username = res.Account.Username;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    
                }
            }
            return Json(new { username = username }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddChat(int idProduct, string message)
        {
            string msg = "";
            string username = Session["userLogin"].ToString();
            ChitChat chat = new ChitChat
            {
                CreatedDate = DateTime.Now,
                Product = db.Products.Find(idProduct),
                Account = db.Accounts.FirstOrDefault(m => m.Username == username),
                Message = message
            };
            try
            {
                db.ChitChats.Add(chat);
                db.SaveChanges();
                msg = "Success";
            }
            catch (DataException e)
            {
                msg = e.Message;
            }

            return Json(new { data = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            ViewBag.IsLoggin = Session["userLogin"] == null ? false : true;
            return View();
        }

        public ActionResult AboutUs()
        {
            ViewBag.IsLoggin = Session["userLogin"] == null ? false : true;
            return View();
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return Redirect("/Home/Index");
        }
    }
}