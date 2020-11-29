using BiddingProject2.DAL;
using BiddingProject2.Models;
using BiddingProject2.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiddingProject2.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        BiddingDbContext db = new BiddingDbContext();
        // GET: Admin/Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSeparateData()
        {
            #region Users on this month
            int usersOnLastMonth = db.Accounts.Count(m => m.CreatedDate.Month == (DateTime.Now.Month - 1 > 0 ? DateTime.Now.Month - 1 : 12) && m.CreatedDate.Year == (DateTime.Now.Month - 1 > 0 ? DateTime.Now.Year : DateTime.Now.Year - 1) && m.RoleId == 2);
            int usersOnThisMonth = db.Accounts.Count(m => m.CreatedDate.Month == DateTime.Now.Month && m.CreatedDate.Year == DateTime.Now.Year && m.RoleId == 2);
            object users = new { thisMonth = usersOnThisMonth };
            if(usersOnLastMonth > 0)
            {
                double difference = usersOnThisMonth > usersOnLastMonth ? CalculateDifference(usersOnThisMonth, usersOnLastMonth) : CalculateDifference(usersOnLastMonth, usersOnThisMonth) ;
                users = new { thisMonth = usersOnThisMonth, lastMonth = usersOnLastMonth, difference = difference };
            }
            #endregion

            #region Products on storage
            int productsOnLastMonth = db.Products.Count(m => m.CreatedDate.Month == (DateTime.Now.Month - 1 > 0 ? DateTime.Now.Month - 1 : 12) && m.CreatedDate.Year == (DateTime.Now.Month -1 > 0 ? DateTime.Now.Year : DateTime.Now.Year -1 )&& !m.State);
            int productsOnThisMonth = db.Products.Count(m => m.CreatedDate.Month == DateTime.Now.Month && m.CreatedDate.Year == DateTime.Now.Year && !m.State);
            object products = new { thisMonth = productsOnThisMonth };
            if(productsOnLastMonth > 0)
            {
                double difference = productsOnThisMonth > productsOnLastMonth ? CalculateDifference(productsOnThisMonth, productsOnLastMonth) : CalculateDifference(productsOnLastMonth, productsOnThisMonth);
                products = new { thisMonth = productsOnThisMonth, lastMonth = productsOnLastMonth, difference = difference };
            }
            #endregion

            #region Sold products
            int soldLastMonth = db.Orders.Where(m => m.CreatedDate.Month == (DateTime.Now.Month - 1 > 0 ? DateTime.Now.Month - 1 : 12) && m.CreatedDate.Year == (DateTime.Now.Month - 1 > 0 ? DateTime.Now.Year : DateTime.Now.Year - 1) && m.State != 3).Count();
            int soldThisMonth = db.Orders.Where(m => m.CreatedDate.Month == DateTime.Now.Month && m.CreatedDate.Year == DateTime.Now.Year && m.State != 3).Count();
            object soldProducts = new { thisMonth = soldThisMonth };
            if(soldLastMonth > 0)
            {
                double difference = soldThisMonth > soldLastMonth ? CalculateDifference(soldThisMonth, soldLastMonth) : CalculateDifference(soldLastMonth, soldThisMonth);
                soldProducts = new { thisMonth = soldThisMonth, lastMonth = soldLastMonth, difference = difference };
            }
            #endregion

            #region Revenues
            var listOrderThisYear = db.Orders.Where(m => m.CreatedDate.Year == DateTime.Now.Year && m.State != 3).ToList();
            List<float> listRevenueThisYear = new List<float>();
            for (int i = 1; i <= 12; i++)
            {
                float totalRevenue = 0;
                foreach (var item in listOrderThisYear)
                {
                    if(item.CreatedDate.Month == i)
                    {
                        totalRevenue += item.Price;
                    }

                }
                listRevenueThisYear.Add(totalRevenue);
            }
            
            var listOrderLastYear = db.Orders.Where(m => m.CreatedDate.Year == (DateTime.Now.Year - 1) && m.State != 3).ToList();
            List<float> listRevenueLastYear = new List<float>();
            for (int i = 1; i <= 12; i++)
            {
                float totalRevenue = 0;
                foreach (var item in listOrderLastYear)
                {
                    if (item.CreatedDate.Month == i)
                    {
                        totalRevenue += item.Price;
                    }

                }
                listRevenueLastYear.Add(totalRevenue);
            }
            #endregion
            return Json(new { users, products, soldProducts, listRevenueThisYear, listRevenueLastYear }, JsonRequestBehavior.AllowGet);
        }

        public double CalculateDifference(int a, int b)
        {
            return ((a - b) / (a * 1.0)) * 100;
        }

        public ActionResult PrivateInfor(string notify = "")
        {
            string username = Session["userLogin"].ToString();
            Account acc = db.Accounts.FirstOrDefault(m => m.Username == username);
            Profile profile = db.Profiles.FirstOrDefault(m => m.ProfileId == acc.AccountId);
            ViewBag.Message = notify;
            return View(profile);
        }

        [HttpPost]
        public ActionResult PrivateInfor(InforModel infor)
        {
            string notify = "";
            Profile prof = db.Profiles.FirstOrDefault(m => m.Account.Username == infor.Username);
            if (prof == null)
            {
                return HttpNotFound();
            }
            try
            {
                if (ModelState.IsValid)
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
                    notify = "Change information successful!";
                    db.SaveChanges();
                }
                else
                {
                    notify = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).First();
                }
                
            }
            catch (Exception e)
            {
                notify="Cannot save. Try again!";
            }
            return RedirectToAction("PrivateInfor", new { notify = notify });
        }

        public ActionResult Category(string msg = "", int page = 1, string searchString = "")
        {
            List<Category> lstCategory = new List<Category>();
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                lstCategory = db.Categories.Where(m => !m.IsDelete && m.Name.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(m => m.CategoryId).Skip(3 * (page - 1)).Take(3).ToList();
                ViewBag.TotalPage = Math.Ceiling(decimal.Parse(db.Categories.Where(m => !m.IsDelete && m.Name.ToUpper().Contains(searchString.ToUpper())).ToList().Count.ToString()) / 3);
            }
            else
            {
                lstCategory = db.Categories.Where(m => !m.IsDelete).OrderByDescending(m => m.CategoryId).Skip(3 * (page - 1)).Take(3).ToList();
                ViewBag.TotalPage = Math.Ceiling(decimal.Parse(db.Categories.Where(m => !m.IsDelete).ToList().Count.ToString()) / 3);
            }
            ViewBag.Msg = msg;
            ViewBag.Search = searchString;
            ViewBag.PageNumber = page;
            return View(lstCategory);
        }

        public ActionResult AddCategory(string msg = "")
        {
            Category cate = new Category();
            ViewBag.Msg = msg;
            return View(cate);
        }

        [HttpPost]
        public ActionResult AddCategory(Category cate)
        {
            int count = 0;
            string msg = "";
            string actionMethod = "AddCategory";
            count = db.Categories.Count(m => m.Name.ToUpper() == cate.Name.ToUpper());
            if (count > 0)
            {
                msg = "This category is duplicated. Please change!";
            }
            else
            {
                
                if (ModelState.IsValid)
                {
                    db.Categories.Add(cate);
                    db.SaveChanges();
                    actionMethod = "Category";
                    msg = "Add Category successful!";
                }
                else
                {
                    msg = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).First();
                } 
            }

            return RedirectToAction(actionMethod, new { msg = msg });

        }

        public ActionResult EditCategory(int idCategory, string msg = "")
        {
            Category cate = db.Categories.Find(idCategory);
            ViewBag.Msg = msg;
            if (cate == null)
            {
                return HttpNotFound();
            }
            return View(cate);
        }

        [HttpPost]
        public ActionResult EditCategory(Category cate)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                Category oldCategory = db.Categories.Find(cate.CategoryId);

                count = db.Categories.Count(m => m.Name.ToUpper() == cate.Name.ToUpper());
                if (oldCategory.Name.ToUpper() != cate.Name.ToUpper() && count == 1)
                {
                    return RedirectToAction("EditCategory", new { idCategory = cate.CategoryId, msg = "This category is duplicated. Please change!" });
                }
                oldCategory.Name = cate.Name;
                db.SaveChanges();
                return RedirectToAction("Category", new { msg = "Edit Category successful!" });
            }
            else
            {
                string msg = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).First();
                return RedirectToAction("EditCategory", new { idCategory = cate.CategoryId, msg = msg });
            }
        }

        [HttpPost]
        public JsonResult DeleteCategory(int categoryId)
        {
            Category cate = db.Categories.Find(categoryId);
            string msg = "Can not delete. Please try again!";
            if (cate != null)
            {
                cate.IsDelete = true;
                db.SaveChanges();
                msg = "Delete successful!";
            }
            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Products(string msg = "", int page = 1, string searchString = "")
        {
            List<Product> products = new List<Product>();
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                products = db.Products.Where(m => m.IsDeleted == false && m.Name.ToUpper().Contains(searchString.ToUpper()))
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(3 * (page - 1)).Take(3).ToList();
                ViewBag.TotalPage = Math.Ceiling(decimal.Parse(db.Products.Where(m => m.IsDeleted == false && m.Name.ToUpper().Contains(searchString.ToUpper())).ToList().Count.ToString()) / 3);
            }
            else
            {
                products = db.Products.Where(m => m.IsDeleted == false)
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(3 * (page - 1)).Take(3).ToList();
                ViewBag.TotalPage = Math.Ceiling(decimal.Parse(db.Products.Where(m => m.IsDeleted == false).ToList().Count.ToString()) / 3);
            }
            ViewBag.Msg = msg;
            ViewBag.Search = searchString;
            ViewBag.PageNumber = page;
            ViewBag.TotalCategory = db.Categories.Count();
            return View(products);
        }

        public ActionResult AddProduct(string msg = "")
        {
            Product prod = new Product();
            ViewBag.Msg = msg;
            ViewBag.ListCategory = db.Categories.Where(m => !m.IsDelete).ToList();
            return View(prod);
        }

        [HttpPost]
        public ActionResult AddProduct(Product prod, int[] idCategories)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (idCategories == null)
                    {
                        return RedirectToAction("AddProduct", new { msg = "Please choose at least one category" });
                    }
                    else if (ModelState.IsValid)
                    {
                        var today = DateTime.Now;
                        if (prod.StartDate.Date < today.Date && prod.StartDate.Month < today.Month)
                        {
                            throw new Exception("Start date can not be begin in the past");
                        }
                        if (prod.BeginPrice < 0)
                        {
                            throw new Exception("The price can not be begin with negative number");
                        }
                        if (prod.ProductImageFile != null)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(prod.ProductImageFile.FileName);
                            string extension = Path.GetExtension(prod.ProductImageFile.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;

                            prod.Image = "~/Images/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/Images/" + fileName));
                            prod.ProductImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            throw new Exception("Image can not be set as default");
                        }
                        prod.CreatedDate = DateTime.Now;
                        prod.EndPrice = prod.BeginPrice;
                        prod.IsDeleted = false;
                        db.Products.Add(prod);

                        if (idCategories != null)
                        {
                            foreach (int item in idCategories)
                            {
                                Category cate = db.Categories.Find(item);
                                ProductCategory prodCate = new ProductCategory() { Product = prod, Category = cate };
                                db.ProductCategories.Add(prodCate);
                            }
                        }
                        db.SaveChanges();
                        int page = db.Products.ToList().Count / 3;
                        msg = "Add product successful!";
                        return RedirectToAction("Products", "Admin", new { msg = msg });
                    }
                }
                else
                {
                    msg = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).First();
                }
                
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return RedirectToAction("AddProduct", new { msg = msg });
        }

        public ActionResult EditProduct(int idProduct, string msg = "")
        {
            int countResult = db.Results.Count(m => m.ProductId == idProduct);
            if(countResult > 0)
            {
                return RedirectToAction("Products", new { msg = "This product is belonged to a customer, You can not edit!" });
            }
            Product prod = db.Products.Find(idProduct);
            List<AssignedCategories> assignedCate = ProductAssigned(idProduct);

            ViewBag.AssignedCate = assignedCate;
            ViewBag.Image = prod.Image;
            ViewBag.Message = msg;
            
            if (prod == null)
            {
                return HttpNotFound();
            }
            return View(prod);
        }

        public List<AssignedCategories> ProductAssigned(int idProduct)
        {
            List<AssignedCategories> assignedCate = new List<AssignedCategories>();

            // Getting list Category in certain product
            var prodCategories = db.ProductCategories.Where(m => m.Product.ProductId == idProduct).ToList();

            foreach (var category in db.Categories.ToList())
            {
                // Add category into new list
                AssignedCategories assigned = new AssignedCategories() { CategoryId = category.CategoryId, CategoryName = category.Name };

                // check condition for each element of category in specific product
                foreach (var item in prodCategories)
                {
                    if(category.CategoryId == item.Category.CategoryId)
                    {
                        assigned.IsAssigned = true;
                    }
                }
                assignedCate.Add(assigned);
            }
            return assignedCate;
        }

        [HttpPost]
        public ActionResult EditProduct(Product prod, int[] idCategories)
        {
            string msg = "Can not edit this product. Try again!";

            List<AssignedCategories> assignedCate = ProductAssigned(prod.ProductId);
            ViewBag.AssignedCate = assignedCate;
            ViewBag.Image = prod.Image;
            try
            {
                if (ModelState.IsValid)
                {
                    Product oldProduct = db.Products.Find(prod.ProductId);
                    var today = DateTime.Now;
                    long myTodayMiliseconds = new DateTimeOffset(today).ToUnixTimeSeconds();
                    long myOldProductMiliseconds = new DateTimeOffset(oldProduct.StartDate).ToUnixTimeSeconds();
                    long myCurrentProductMilisecond = 0;
                    if (prod.StartDate != null)
                    {
                        myCurrentProductMilisecond = new DateTimeOffset(prod.StartDate).ToUnixTimeSeconds();
                    }
                    if (myCurrentProductMilisecond != 0 && myOldProductMiliseconds != myCurrentProductMilisecond && myCurrentProductMilisecond < myTodayMiliseconds)
                    {
                        throw new Exception("Start date can not be begin in the past");
                    }
                    if (prod.BeginPrice < 0)
                    {
                        throw new Exception("The price can not be begin with negative number");
                    }
                    string[] updateData = new string[] { "Name", "BeginPrice", "StartDate", "State", "Description" };
                    if (prod.ProductImageFile != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(prod.ProductImageFile.FileName);
                        string extension = Path.GetExtension(prod.ProductImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;

                        prod.Image = "~/Images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Images/" + fileName));
                        prod.ProductImageFile.SaveAs(fileName);
                        oldProduct.Image = prod.Image;
                    }

                    if (TryUpdateModel(oldProduct, updateData))
                    {
                        // Getting old list Category
                        var oldList = db.ProductCategories.Where(m => m.Product.ProductId == prod.ProductId).ToList();
                        foreach (var item in oldList)
                        {
                            db.ProductCategories.Remove(item);
                        }
                        for (int i = 0; i < idCategories.Length; i++)
                        {
                            db.ProductCategories.Add(new ProductCategory() { Product = oldProduct, Category = db.Categories.Find(idCategories[i]) });
                        }
                        db.SaveChanges();
                        msg = "Edit product successful!";
                        return RedirectToAction("Products", "Admin", new { msg = msg });
                    }
                }
                else
                {
                    msg = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).First();
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return RedirectToAction("EditProduct", new { msg = msg });
        }

        [HttpPost]
        public ActionResult DeleteProducts(int idProduct)
        {
            string msg = "Can not delete. Please try again!";
            int countResult = db.Results.Count(m => m.ProductId == idProduct);
            if (countResult > 0)
            {
                msg = "This product is belonged to a customer, You can not edit!";
            }
            else
            {
                Product prod = db.Products.Find(idProduct);

                if (prod != null)
                {
                    prod.IsDeleted = true;
                    db.SaveChanges();
                    msg = "Delete successful!";
                }
            }
            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Customers(string msg = "", int page = 1, string searchString = "")
        {
            List<Account> customers = new List<Account>();
            if (!String.IsNullOrWhiteSpace(searchString)){
                customers = db.Accounts
                    .Where(m => m.Role.RoleId == 2 && !m.IsDeleted)
                    .Where(m => new[] { m.Username.ToUpper(), m.Profile.Name.ToUpper()}.Any(x => x.Contains(searchString.ToUpper())))
                    .OrderByDescending(m => m.AccountId)
                    .Skip(3 * (page - 1)).Take(3)
                    .ToList();
            }
            else
            {
                customers = db.Accounts.Where(m => m.Role.RoleId == 2)
                    .OrderByDescending(m => m.AccountId)
                    .Skip(3 * (page - 1)).Take(3)
                    .ToList();
            }
            ViewBag.Msg = msg;
            ViewBag.TotalPage = Math.Ceiling(decimal.Parse(db.Accounts.Where(m => m.Role.RoleId == 2).ToList().Count.ToString()) / 3);
            ViewBag.PageNumber = page;
            return View(customers);
        }

        [HttpGet]
        public JsonResult ResetPassword(int idCustomer)
        {
            Account customer = db.Accounts.Find(idCustomer);
            string msg = "Can not reset password";
            if (customer != null && customer.RoleId == 2)
            {
                customer.Password = "123";
                db.SaveChanges();
                msg = "Reset password successful to \"123\"";
            }
            return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteCustomer(int idCustomer)
        {
            Account customer = db.Accounts.Find(idCustomer);
            string msg = "Can not delete this customer";
            if(customer != null && customer.RoleId == 2)
            {
                customer.IsDeleted = true;
                db.SaveChanges();
                msg = "Delete customer successful!";
            }
            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Orders(string searchString, int page = 1)
        {
            List<Order> orders = db.Orders.OrderByDescending(m => m.OrderId).Take(3).Skip(3 * (page - 1)).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = db.Orders.Where(m => new[] { m.Product.Name.ToUpper(), m.Number, m.Account.Profile.Name.ToUpper() } .Any(x => x.Contains(searchString.ToUpper())))
                    .OrderByDescending(m => m.OrderId)
                    .Take(3).Skip(3 * (page - 1))
                    .ToList();
            }
            ViewBag.TotalPage = Math.Ceiling(decimal.Parse(db.Orders.Count().ToString()) / 3);
            ViewBag.PageNumber = page;
            ViewBag.Search = searchString;
            return View(orders);
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return Redirect("/Home/Index");
        }
    }
}