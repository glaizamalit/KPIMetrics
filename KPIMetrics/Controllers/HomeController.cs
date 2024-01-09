using KPIMetrics.Models;
using KPIMetrics.Sso;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPIMetrics.Controllers
{
    public class HomeController : Controller
    {
        string userName = "";
        Models.KPIMetrics context = new Models.KPIMetrics();

        [SsoAuth]
        public ActionResult Index()
        {
            try
            {               
                User user = GetUser();
                string controller = "";
                string action = "";

                if (user != null)
                {
                    LogManager.Log.Info("Redirecting to Index view...");
                    var userRec = (from u in context.Users
                                   join ul in context.UserLocations on u.Initials equals ul.Initials
                                   join l in context.BULocations on ul.BULocID equals l.ID
                                   where u.Initials == userName
                                   select new UserInfo
                                   {
                                       Initial = u.Initials,
                                       BUId = l.BUID,
                                       LocationId = ul.BULocID
                                   }).ToList();

                    if (userRec.Count > 0)
                    {
                        int[] buids = userRec.Select(b => b.BUId).ToArray();

                        if (buids.Contains(0) || buids.Contains(1))
                        {
                            action = "Management";
                            controller = "Commercial";
                        }
                        else if (buids.Contains(2))
                        {
                            action = "Lifeboat";
                            controller = "Seasafe";
                        }
                        else if (buids.Contains(3))
                        {
                            action = "Cargolux";
                            controller = "Agency";
                        }
                        else if (buids.Contains(4))
                        {
                            action = "List";
                            controller = "HR";
                        }
                        else
                        {
                            action = "Mgt";
                            controller = "Shipmanagement";
                        }

                            TempData["Role"] = string.Join(",", buids);
                        Session["Role"] = string.Join(",", buids);
                    }
                    else
                    {
                        LogManager.Log.Info("Username " + userName + " is not authorized user...");
                        Session["Role"] = "";
                        TempData["Role"] = "";
                        return View("Unauthorized");
                    }

                    return RedirectToAction(action, controller);
                }
                else
                {
                    LogManager.Log.Info("Username " + userName + " is not authorized user...");
                    Session["Role"] = "";
                    TempData["Role"] = "";
                    return View("Unauthorized");
                }
            }
            catch (Exception ex)
            {
                LogManager.Log.Info(ex.Message);
                throw;
            }
        }

        public User GetUser()
        {                   
            User userRec = new User();
            userRec = null;
            userName = Session["tkusername"] != null ? Session["tkusername"].ToString() : null;
            LogManager.Log.Info("Username " + userName + " has logged in...");
            var user = (from u in context.Users
                        where u.Initials == userName
                        select u).ToList();
            if (user.Count() > 0)
            {
                userRec = user.FirstOrDefault();
            }

            return userRec;
        }

        // GET: Logout
        public ActionResult Logout()
        {
            LogManager.Log.Info("Username " + Session["tkusername"].ToString() + " has logged out...");
            Session["isAdmin"] = false;
            Session["login"] = null;
            Session["tkusername"] = null;
            Session["tkid"] = null;
            return Redirect(ConfigurationManager.AppSettings["logout_url"]);
        }

        [SsoAuth]
        public ActionResult About()
        {
            return View();
        }

    }
}
