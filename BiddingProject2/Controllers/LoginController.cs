using BiddingProject2.DAL;
using BiddingProject2.ExtendedClasses;
using BiddingProject2.Models;
using BiddingProject2.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BiddingProject2.Controllers
{
    public class LoginController : Controller
    {
        BiddingDbContext db = new BiddingDbContext();
        // GET: Login
        public ActionResult Index(string msg = "")
        {
            ViewBag.MessRegist = msg;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            var b = db.Accounts.ToList();
            Account acc = db.Accounts.FirstOrDefault(a => a.Username == username && a.Password == password && a.IsDeleted == false);
            if (acc != null)
            {
                FormsAuthentication.SetAuthCookie(username, false);
                Session["userLogin"] = username;
                MyRoleProvider getRole = new MyRoleProvider();
                var role = getRole.GetRolesForUser(username);
                if (role.Contains("Administrator"))
                {
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else if (role.Contains("Customer"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", new { msg = "Invalid account" });
        }

        public ActionResult RegisterAccount(string msg = "")
        {
            ViewBag.MessRegist = msg;
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAccount(RegisterModel model)
        {
            if(model.Password != model.RePassword)
            {
                return RedirectToAction("RegisterAccount", new { msg = "Repassword is not matched. Please check!" });
            }
            Account acc = db.Accounts.SingleOrDefault(m => m.Username.ToUpper() == model.Username.ToUpper());
            Account checkEmail = db.Accounts.SingleOrDefault(m => m.Profile.Email.ToUpper().Equals(model.Email.ToUpper()));
            if (acc != null)
            {
                return RedirectToAction("RegisterAccount", new { msg = "This account is existed in this system. Please change!" });
            }
            else if(checkEmail != null)
            {
                return RedirectToAction("RegisterAccount", new { msg = "This email is existed in this system. Please change!" });
            }
            else
            {
                Account newAcc = new Account()
                {
                    Username = model.Username,
                    Password = model.Password,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now,
                    RoleId = 2
                };

                Profile prof = new Profile()
                {
                    Account = newAcc,
                    AccountId = newAcc.AccountId,
                    Email = model.Email,
                    Name = model.DisplayName
                };

                db.Accounts.Add(newAcc);

                db.Profiles.Add(prof);
                db.SaveChanges();
                return RedirectToAction("Index", new { msg = "Register successful. Please login!"});
            }
        }
    }
}