using KPIMetrics.Models;
using KPIMetrics.Sso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPIMetrics.Controllers
{
    public class UsersController : Controller
    {
        string userName = "";
        Models.KPIMetrics db = new Models.KPIMetrics();

        [SsoAuth]
        // GET: Users
        public ActionResult List()
        {
            try
            {
                userName = Session["tkusername"].ToString();
                UserInfo userRec = new UserInfo();
                var user = (from u in db.Users
                            join ul in db.UserLocations on u.Initials equals ul.Initials
                            join bl in db.BULocations on ul.BULocID equals bl.ID
                            where u.Initials == userName
                            select new UserInfo
                            {
                                Initial = u.Initials,
                                BUId = bl.BUID,
                                LocationId = ul.ID
                            }).ToList();
                if (user.Count() > 0)
                {
                    userRec = user.FirstOrDefault();
                }

                if (userRec.Initial != null)
                {
                    LogManager.Log.Info("Redirecting to Index view...");
                   // Session["Role"] = db.KPI_BUs.Where(b => b.ID == userRec.BUId).Select(b => b.Name).FirstOrDefault();

                    UserInfo userInfo = new UserInfo();
                    userInfo.BUList = GetBU("");
                    userInfo.LocationList = GetLocation("");
                    return View("List", userInfo);
                }
                else
                {
                    LogManager.Log.Info("Username " + userName + " is not authorized user...");
                    Session["Role"] = "";
                    return View("../Home/Unauthorized");
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        public JsonResult UserList()
        {
            userName = Session["tkusername"].ToString();
            List<UserInfo> users = (from u in db.Users
                                    join ul in db.UserLocations on u.Initials equals ul.Initials
                                    join l in db.BULocations on ul.BULocID equals l.ID                                 
                                    select new UserInfo
                                    {
                                        Id = ul.ID,
                                        Initial = u.Initials,
                                        FullName = u.Name,
                                        BUId = l.BUID,
                                        BU = db.KPI_BUs.Where(b=>b.ID == l.BUID).Select(b=>b.Name).FirstOrDefault(),
                                        LocationId = ul.BULocID,
                                        Location = db.BULocations.Where(o=>l.ID == ul.BULocID).Select(o=>l.Location).FirstOrDefault(),
                                        IsActive = u.IsActive,
                                        UpdatedBy = u.UpdatedBy,
                                        UpdatedDt = u.UpdatedDt
                                    }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UserInfo user)
        {
            try
            {
                var uRec = db.Users.Where(u => u.Initials == user.Initial).Select(u => u).ToList();
                if (uRec.Count == 0)
                {
                    User userRec = new User();
                    userRec.Initials = user.Initial;
                    userRec.Name = user.FullName;
                    userRec.IsActive = true;
                    userRec.CreatedBy = Session["tkusername"].ToString();
                    userRec.CreatedDt = DateTime.Now;
                    db.Users.Add(userRec);
                }

                var uLocRec = db.UserLocations.Where(u => u.Initials == user.Initial && u.BULocID == user.LocationId).Select(u => u).ToList();

                UserLocation usr = new UserLocation();
                if (uLocRec.Count == 0)
                {
                    usr.Initials = user.Initial;
                    usr.BULocID = user.LocationId;
                    db.UserLocations.Add(usr);
                }

                db.SaveChanges();
                LogManager.Log.Info("User " + usr.Initials + " successfully added.");

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult Create(UserInfo user)
        {
            try
            {
                var uRec = db.Users.Where(u => u.Initials == user.Initial).Select(u => u).ToList();
                if (uRec.Count == 0)
                {
                    User userRec = new User();
                    userRec.Initials = user.Initial;
                    userRec.Name = user.FullName;
                    userRec.IsActive = true;
                    userRec.CreatedBy = Session["tkusername"].ToString();
                    userRec.CreatedDt = DateTime.Now;
                    db.Users.Add(userRec);
                }

                int userLoc = int.Parse(user.Location);
                int bu = db.BULocations.Where(u => u.ID == userLoc).Select(u => u.BUID).FirstOrDefault();
                var uLocRec = db.UserLocations.Where(u => u.Initials == user.Initial && u.BULocID == userLoc).Select(u => u).ToList();

                UserLocation usr = new UserLocation();
                if (uLocRec.Count == 0)
                {
                    usr.Initials = user.Initial;
                    usr.BULocID = userLoc;
                    db.UserLocations.Add(usr);
                }

                db.SaveChanges();

                user.Location = db.BULocations.Where(u => u.ID == userLoc).Select(u => u.Location).FirstOrDefault();
                user.BU = db.KPI_BUs.Where(b => b.ID == bu).Select(b => b.Name).FirstOrDefault();
                LogManager.Log.Info("User " + usr.Initials + " successfully added.");

                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }


        public JsonResult Update(UserInfo user)
        {
            try
            {
                User usr = db.Users.Where(u => u.Initials == user.Initial).Select(u => u).FirstOrDefault();
                usr.Name = user.FullName;
                usr.IsActive = user.IsActive;
                usr.UpdatedBy = Session["tkusername"].ToString();
                usr.UpdatedDt = DateTime.Now;

                db.Users.Attach(usr);

                db.Entry(usr).Property(u => u.Name).IsModified = true;
                db.Entry(usr).Property(u => u.IsActive).IsModified = true;
                db.Entry(usr).Property(u => u.UpdatedBy).IsModified = true;
                db.Entry(usr).Property(u => u.UpdatedDt).IsModified = true;

                UserLocation userLoc = db.UserLocations.Where(l => l.ID == user.Id).Select(l => l).FirstOrDefault();
                userLoc.BULocID = user.LocationId;

                db.UserLocations.Attach(userLoc);
                db.Entry(userLoc).Property(u => u.BULocID).IsModified = true;

                db.SaveChanges();
                LogManager.Log.Info("User " + user.Initial + " successfully updated.");
                //  user.BU = db.KPI_BUs.Where(r => r.ID == usr.BUId).Select(r => r.Name).FirstOrDefault();

                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult CheckDuplication(string initial, string name, string location, string isactive, string oldinitial, string oldname, string oldlocation, string oldisactive)
        {
            try
            {
                bool isValid = false;

                if (initial == oldinitial && name == oldname && location == oldlocation && isactive == oldisactive)
                {
                    isValid = true;
                }
                else
                {
                    if (location != "")
                    {
                        int loc = int.Parse(location);
                        bool active = bool.Parse(isactive);
                        var users = (from u in db.Users
                                     join ul in db.UserLocations on u.Initials equals ul.Initials
                                     where u.Initials == initial && u.Name == name && ul.BULocID == loc && u.IsActive == active
                                     select new { u, ul.BULocID }).ToList();

                        if (users.Count() == 0)
                        {
                            isValid = true;
                        }
                    }
                }

                return Json(isValid.ToString().ToUpper(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }


        public SelectList GetBU(string SelectedValue)
        {
            List<KPI_BU> roleList = new List<KPI_BU>();
            var roles = db.KPI_BUs.Select(s => s);

            foreach (var a in roles)
            {
                roleList.Add(a);
            }

            return new SelectList(roleList.ToList(), "ID", "Name", SelectedValue);
        }

        public SelectList GetLocation(string SelectedValue)
        {
            List<BULocation> locationList = new List<BULocation>();
            var loc = db.BULocations.Select(s => s);

            foreach (var a in loc)
            {
                locationList.Add(a);
            }

            return new SelectList(locationList.ToList(), "ID", "Location", SelectedValue);
        }


        public JsonResult GetBUList(string SelectedValue)
        {
            var bus = (from bu in db.KPI_BUs
                       from b in db.BULocations.DefaultIfEmpty() 
                       select new { BUId = bu.ID, bu.Name }).Distinct().ToList();

            return Json(bus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationList()
        {
            var loc = (from bu in db.BULocations
                       join b in db.KPI_BUs on bu.BUID equals b.ID
                       select new { BUId = b.ID, b.Name, LocationId = bu.ID, bu.Location }).OrderBy(b=>b.Location).ToList();

            return Json(loc, JsonRequestBehavior.AllowGet);
        }
    }
}