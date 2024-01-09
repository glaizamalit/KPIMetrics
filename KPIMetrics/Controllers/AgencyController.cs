using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KPIMetrics.Sso;
using KPIMetrics.Models;
using System.Globalization;
using System.Configuration;

namespace KPIMetrics.Controllers
{
    public class AgencyController : Controller
    {
        string userName = "";
        string BU = "";
        Models.KPIMetrics db = new Models.KPIMetrics();
        KPIHelper helper = new KPIHelper();

        // GET: Agency
        [SsoAuth]
        public ActionResult Index()
        {
            try
            {
                userName = Session["tkusername"].ToString();
                BU = db.KPI_BUs.Where(b => b.ID == 3).Select(b => b.Name).FirstOrDefault();
              
                var user = (from u in db.Users
                            join ul in db.UserLocations on u.Initials equals ul.Initials
                            join l in db.BULocations on ul.BULocID equals l.ID
                            where u.Initials == userName
                            select new UserInfo
                            {
                                Initial = u.Initials,
                                BUId = l.BUID,
                                LocationId = ul.BULocID,
                                Location = l.Location
                            }).ToList();

                if (user.Count() > 0)
                {
                    int[] buids = user.Select(b => b.BUId).ToArray();
                    string[] locs = user.Where(l=> l.Initial == userName && (l.BUId == 0 || l.BUId == 3)).Select(b => b.Location).ToArray();
                    LogManager.Log.Info("Redirecting to Index view...");

                    Session["Location"] = string.Join(",", locs);
                    Session["Role"] = string.Join(",", buids);

                    //Session["Role"] = role;
                    //Session["Location"] = userRec.Location;                   

                    ViewBag.LF = Session["LF"];
                    ViewBag.KPI = Session["MetricName"];
                    string metricName = Session["MetricName"].ToString();

                    var kpis = (from k in db.KPI_Metrics
                                where k.MetricName == metricName
                                select k).ToList();                                                 

                    KPIClass kpi = new KPIClass();
                    kpi.KPI1List = GetKPI1("");
                    kpi.KPI2List = GetKPI2("");
                    kpi.KPI3List = GetKPI3("");
                    kpi.ModelList = GetReference(0, "Model", "");
                    kpi.MonthList = GetMonth("");
                    kpi.LineFunctionList = helper.getLineFunction(BU, metricName);

                    if (kpis.Count() > 0)
                    {
                        var k = kpis.FirstOrDefault();
                        Session["KPI1"] = k.KPI1;
                        Session["KPI2"] = k.KPI2;
                        Session["KPI3"] = k.KPI3;
                        Session["KPI4"] = k.KPI4;

                        kpi.KPI1 = k.KPI1;
                        kpi.KPI2 = k.KPI2;
                        kpi.KPI3 = k.KPI3;
                        kpi.KPI4 = k.KPI4;
                    }
                    return View("Index", kpi);
                }
                else
                {
                    LogManager.Log.Info("Username " + userName + " is not authorized user...");
                    //Session["Role"] = "";
                    return View("../Home/Unauthorized");
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        [SsoAuth]
        public ActionResult Cargolux()
        {
            int metricId = (from l in db.KPI_Metrics
                            where l.MetricName == "Cargolux"
                            select l.ID).FirstOrDefault();

            string lineFunction = (from l in db.KPI_Line_Functions
                                   where l.BUId == 3 && l.MetricId == metricId
                                   select l.Name).FirstOrDefault();            

            Session["LF"] = lineFunction;
            Session["MetricName"] = "Cargolux";
            return RedirectToAction("Index");
        }

        [SsoAuth]
        public ActionResult PortCall()
        {
            Session["LF"] = "";
            Session["MetricName"] = "Port Call";
            return RedirectToAction("Index");
        }

        [SsoAuth]
        public ActionResult CrewChange()
        {
            Session["LF"] = "";
            Session["MetricName"] = "Crew Change";
            return RedirectToAction("Index");
        }

        public JsonResult List()
        {
            string location = Session["Location"].ToString();
            string[] LOB = null;
            string metricName = Session["MetricName"].ToString();
            int metricId = db.KPI_Metrics.Where(m => m.MetricName == metricName).Select(m => m.ID).FirstOrDefault();

            LOB = helper.getLineFunctionNames(3, metricName);

            List<KPIClass> comm = new List<KPIClass>();
            var accounts = (from o in db.KPI_Agencies
                            where LOB.Contains(o.LOB) && o.MetricID == metricId
                            select new KPIClass
                            {
                                MetricId = o.MetricID,
                                SetId = o.SetID,
                                BU = o.BU,
                                LOB = o.LOB,
                                CalYear = o.CalYear,
                                CalMonth = o.CalMonth,
                                FinYear = o.FinYear,
                                KPI1 = o.KPI1,
                                KPI2 = o.KPI2,
                                KPI3 = o.KPI3,
                                KPI4 = o.KPI4,
                                BGT = db.KPI_Agencies.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                ACT = db.KPI_Agencies.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                RF1 = db.KPI_Agencies.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                RF2 = db.KPI_Agencies.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                RF3 = db.KPI_Agencies.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                //UpdatedBy = o.UpdatedBy,
                                //CreatedDt = o.CreatedDt
                            }).Distinct().AsEnumerable();

            if (location != "All" && Session["MetricName"].ToString() != "Cargolux")
            {
                accounts = accounts.Where(a => a.KPI3 == location).Select(a => a).AsEnumerable();
            }

            accounts = accounts.Select(o => new KPIClass
            {
                MetricId = o.MetricId,
                SetId = o.SetId,
                BU = o.BU,
                LOB = o.LOB,
                CalYear = o.CalYear,
                CalMonth = o.CalMonth,
                CalMonth2 = DateTime.Parse(o.CalYear + "-" + o.CalMonth + "-" + "01"),
                FinYear = o.FinYear,
                KPI1 = o.KPI1,
                KPI2 = o.KPI2,
                KPI3 = o.KPI3,
                KPI4 = o.KPI4,
                BGT = db.KPI_Agencies.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                ACT = db.KPI_Agencies.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF1 = db.KPI_Agencies.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF2 = db.KPI_Agencies.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF3 = db.KPI_Agencies.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                //UpdatedBy = o.UpdatedBy,
                //CreatedDt = o.CreatedDt
            }).Distinct().ToList();

            foreach (var item in accounts)
            {
                KPIClass kpi = new KPIClass();
                kpi.Id = db.KPI_Agencies.Where(c => c.BU == item.BU && c.LOB == item.LOB && c.CalMonth == item.CalMonth && c.CalYear == item.CalYear && c.KPI1 == item.KPI1 && c.KPI2 == item.KPI2 && c.KPI3 == item.KPI3 && c.KPI4 == item.KPI4 && c.SetID == item.SetId).Select(c => c.ID).FirstOrDefault();
                kpi.SetId = item.SetId;
                kpi.MetricId = item.MetricId;
                kpi.BU = item.BU;
                kpi.LOB = item.LOB;
                kpi.CalYear = item.CalYear;
                kpi.CalMonth = item.CalMonth;
                kpi.CalMonth2 = item.CalMonth2;
                kpi.FinYear = item.FinYear;
                kpi.KPI1 = item.KPI1;
                kpi.KPI2 = item.KPI2;
                kpi.KPI3 = item.KPI3;
                kpi.KPI4 = item.KPI4;
                kpi.BGT = item.BGT;
                kpi.ACT = item.ACT;
                kpi.RF1 = item.RF1;
                kpi.RF2 = item.RF2;
                kpi.RF3 = item.RF3;
                //kpi.CreatedDt = item.CreatedDt;

                comm.Add(kpi);
            }

            return Json(comm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPIClass kpi)
        {
            try
            {
                DateTime dateNow = DateTime.Now;
                string metricName = Session["MetricName"].ToString();
                int calMonth = dateNow.Month;


                //logic for months               
                List<int> months = new List<int>();
                var models = (from m in db.KPI_References
                              where m.RefType == "Model"
                              select m.RefValue).ToList();

                for (int i = kpi.From; i <= kpi.To; i++)
                {
                    months.Add(i);
                }


                foreach (var month in months)
                {
                    int setId = db.KPI_Agencies.Count() == 0 ? 1 : db.KPI_Agencies.Max(c => c.SetID) + 1;
                    foreach (var model in models)
                    {
                        KPI_Agency al = new KPI_Agency();
                        al.SetID = setId;
                        al.MetricID = db.KPI_Metrics.Where(k => k.MetricName == metricName).Select(k => k.ID).FirstOrDefault();
                        al.BU = BU = db.KPI_BUs.Where(b => b.ID == 3).Select(b => b.Name).FirstOrDefault();
                        if (Session["LF"].ToString() != "")
                        {
                            al.LOB = Session["LF"].ToString();
                        }
                        else
                        {
                            int lineFunction = int.Parse(kpi.LOB);
                            al.LOB = db.KPI_Line_Functions.Where(l => l.ID == lineFunction).Select(l => l.Name).FirstOrDefault();
                        }

                        al.CalYear = month <= 3 ? int.Parse(kpi.CalYear).ToString() : (int.Parse(kpi.CalYear) + 1).ToString();
                        al.CalMonth = GetMonthAbbr(month);
                        al.FinYear = kpi.CalYear + "-" + (int.Parse(kpi.CalYear) + 1).ToString().Substring(2, 2);
                        al.Model = model;
                        al.KPI1 = kpi.KPI1;
                        al.KPI2 = kpi.KPI2;
                        al.KPI3 = kpi.KPI3;
                        al.KPI4 = kpi.KPI4;
                        al.Measure1 = 0;
                        al.Measure2 = (decimal?)null;
                        al.CreatedBy = Session["tkusername"].ToString();
                        al.CreatedDt = dateNow;

                        var agency = (from c in db.KPI_Agencies
                                      where c.MetricID == al.MetricID && c.BU == al.BU && c.LOB == al.LOB && c.CalYear == al.CalYear && c.CalMonth == al.CalMonth && c.FinYear == al.FinYear && c.Model == model && c.KPI1 == al.KPI1 && c.KPI2 == al.KPI2 && c.KPI3 == al.KPI3 && c.KPI4 == al.KPI4
                                      select c).ToList();

                        if (agency.Count == 0)
                        {
                            db.KPI_Agencies.Add(al);
                            db.SaveChanges();
                            LogManager.Log.Info("Agency Id " + al.ID + " was successfully added by " + Session["tkusername"].ToString() + ".");

                            KPI_metric_log log = new KPI_metric_log();
                            log.LogActionType = "New";
                            log.LogDatetime = DateTime.Now;
                            log.LogSource = "Add " + Session["LF"];
                            log.LogTableID = al.ID;
                            log.LogTablename = "KPI_Agency";
                            log.LogUser = Session["tkusername"].ToString();

                            db.KPI_metric_logs.Add(log);
                            db.SaveChanges();

                            LogManager.Log.Info("Agency Id " + al.ID + " was successfully added to the database log.");
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult SaveChanges(List<KPIClass> kpi)
        {
            var msge = "";
            try
            {
                foreach (var item in kpi)
                {
                    Update(item);
                }
                msge = "Success";

            }
            catch (Exception ex)
            {
                msge = ex.Message;
            }

            return Json(msge);
        }

        public JsonResult Update(KPIClass kpi)
        {
            try
            {
                DateTime dateNow = DateTime.Now;
                BU = db.KPI_BUs.Where(b => b.ID == 3).Select(b => b.Name).FirstOrDefault();
                var models = (from m in db.KPI_References
                              where m.RefType == "Model"
                              select m.RefValue).ToList();

                var ac = db.KPI_Agencies.Where(u => u.BU == BU && u.LOB == kpi.LOB && u.KPI1 == kpi.KPI1 && u.KPI2 == kpi.KPI2 && u.KPI3 == kpi.KPI3 && u.KPI4 == kpi.KPI4 && u.CalMonth == kpi.CalMonth && u.CalYear == kpi.CalYear && u.SetID == kpi.SetId).Select(u => u).ToList();
                KPI_Agency al = new KPI_Agency();

                if (ac.ToList().Count() > 0)
                {
                    foreach (var model in models)
                    {
                        al = ac.Where(m => m.Model == model).FirstOrDefault();

                        var agency = (from c in db.KPI_Agencies
                                      where c.MetricID == al.MetricID && c.BU == al.BU && c.LOB == al.LOB && c.CalYear == al.CalYear && c.CalMonth == al.CalMonth && c.FinYear == al.FinYear && c.Model == model && c.KPI1 == al.KPI1 && c.KPI2 == al.KPI2 && c.KPI3 == al.KPI3 && c.KPI4 == al.KPI4 && c.SetID == al.SetID
                                      select c).ToList();

                        if (agency.Count == 0 && model != "")
                        {
                            string metricName = Session["MetricName"].ToString();

                            KPI_Agency k = new KPI_Agency();
                            k.MetricID = db.KPI_Metrics.Where(m => m.MetricName == metricName).Select(m => k.ID).FirstOrDefault();
                            k.SetID = al.SetID;
                            k.BU = BU = db.KPI_BUs.Where(b => b.ID == 3).Select(b => b.Name).FirstOrDefault();
                            k.LOB = Session["LF"].ToString();
                            k.CalYear = al.CalYear;
                            k.CalMonth = al.CalMonth;
                            k.FinYear = al.FinYear;
                            k.Model = model;
                            k.KPI1 = al.KPI1;
                            k.KPI2 = al.KPI2;
                            k.KPI3 = al.KPI3;
                            k.KPI4 = al.KPI4;
                            k.Measure1 = model == "ACT" ? kpi.ACT : (model == "RF1" ? kpi.RF1 : (model == "RF2" ? kpi.RF2 : (model == "BGT" ? kpi.BGT : kpi.RF3)));
                            k.Measure2 = (decimal?)null;
                            k.CreatedBy = Session["tkusername"].ToString();
                            k.CreatedDt = dateNow;


                            db.KPI_Agencies.Add(k);
                            db.SaveChanges();
                            LogManager.Log.Info("Agency Id " + al.ID + " successfully added.");
                        }
                        else
                        {
                            string oldValue = al.Measure1.ToString();
                            al.Measure1 = model == "ACT" ? kpi.ACT : (model == "RF1" ? kpi.RF1 : (model == "RF2" ? kpi.RF2 : (model == "BGT" ? kpi.BGT : kpi.RF3)));
                            al.UpdatedBy = Session["tkusername"].ToString();
                            al.UpdatedDt = dateNow;

                            db.KPI_Agencies.Attach(al);

                            db.Entry(al).Property(u => u.Measure1).IsModified = true;
                            db.Entry(al).Property(u => u.UpdatedBy).IsModified = true;
                            db.Entry(al).Property(u => u.UpdatedDt).IsModified = true;

                            db.SaveChanges();
                            LogManager.Log.Info("Measure1 for Agency Id " + kpi.Id + " was successfully updated by " + Session["tkusername"].ToString() + ".");


                            if (al.Measure1 != null && oldValue != al.Measure1.Value.ToString("0.0000"))
                            {
                                KPI_metric_log log = new KPI_metric_log();
                                log.LogActionType = "Edit";
                                log.LogDatetime = DateTime.Now;
                                log.LogSource = "Update " + Session["LF"];
                                log.LogTableID = kpi.Id;
                                log.LogTablename = "KPI_Agency";
                                log.LogUser = Session["tkusername"].ToString();

                                db.KPI_metric_logs.Add(log);
                                db.SaveChanges();

                                KPI_metric_log_detail logDetail = new KPI_metric_log_detail();
                                logDetail.LogdFieldname = model;
                                logDetail.LogdNewvalue = al.Measure1.Value.ToString("0.0000");
                                logDetail.LogdOldvalue = oldValue;
                                logDetail.LogID = log.ID;

                                db.KPI_metric_log_details.Add(logDetail);
                                db.SaveChanges();

                                LogManager.Log.Info("Agency Id " + al.ID + " successfully added to the database log.");
                            }
                        }
                    }
                }

                kpi.CalMonth2 = DateTime.Parse(kpi.CalYear + "-" + kpi.CalMonth + "-01");

                return Json(kpi, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        private SelectList GetKPI1(string SelectedValue)
        {
            List<string> typeLst = new List<string>();
            string metricName = Session["MetricName"].ToString();
            string kpi1 = db.KPI_Metrics.Where(k => k.BUID == 3 && k.MetricName == metricName).Select(k => k.KPI1).FirstOrDefault();

            var shipType = db.KPI_References.Where(s => s.RefType == kpi1).Select(s => s.RefValue);

            foreach (var a in shipType)
            {
                typeLst.Add(a);
            }


            return new SelectList(typeLst.ToList(), SelectedValue);
        }

        private SelectList GetKPI2(string SelectedValue)
        {
            List<string> typeLst = new List<string>();
            string metricName = Session["MetricName"].ToString();
            string kpi2 = db.KPI_Metrics.Where(k => k.BUID == 3 && k.MetricName == metricName).Select(k => k.KPI2).FirstOrDefault();

            var shipType = db.KPI_References.Where(s => s.RefType == kpi2).Select(s => s.RefValue);
            foreach (var a in shipType)
            {
                typeLst.Add(a);
            }

            return new SelectList(typeLst.ToList(), SelectedValue);
        }

        private SelectList GetKPI3(string SelectedValue)
        {
            List<string> typeLst = new List<string>();
            string metricName = Session["MetricName"].ToString();
            string kpi3 = db.KPI_Metrics.Where(k => k.BUID == 3 && k.MetricName == metricName).Select(k => k.KPI3).FirstOrDefault();


            var shipType = db.KPI_References.Where(s => s.RefType == kpi3).Select(s => s.RefValue);

            foreach (var a in shipType)
            {
                typeLst.Add(a);
            }

            return new SelectList(typeLst.ToList(), SelectedValue);
        }


        private SelectList GetReference(int BU, string refType, string SelectedValue)
        {
            List<string> typeLst = new List<string>();
            var shipType = db.KPI_References.Where(s => s.BUID == BU && s.RefType == refType).Select(s => s.RefValue);

            foreach (var a in shipType)
            {
                typeLst.Add(a);
            }

            return new SelectList(typeLst.ToList(), SelectedValue);
        }

        //private SelectList GetCustomers(string SelectedValue)
        //{
        //    List<string> customerList = new List<string>();
        //    var model = db.KPI_Agencies.Select(s => s.KPI2).Distinct();

        //    foreach (var a in model)
        //    {
        //        customerList.Add(a);
        //    }

        //    return new SelectList(customerList.ToList(), SelectedValue);
        //}


        private SelectList GetMonth(string SelectedValue)
        {
            List<SelectListItem> monthList = new List<SelectListItem>();
            monthList = helper.getMonth(SelectedValue);

            return new SelectList(monthList.ToList(), "Value", "Text", SelectedValue);
        }

        private string GetMonthAbbr(int month)
        {
            string monthAbbr = helper.getMonthAbbr(month);
            return monthAbbr;
        }

        public JsonResult GetMonthYearRec(string customer, string shipname, string calMonth, string calYear)
        {
            string[] LOB = null;
            string metricName = Session["MetricName"].ToString();
            string location = Session["Location"].ToString();

            int metricId = (from m in db.KPI_Metrics
                            where m.MetricName == metricName
                            select m.ID).FirstOrDefault();

            LOB = helper.getLineFunctionNames(3, metricName);

            var kpi = (from o in db.KPI_Agencies
                       where o.CalMonth == calMonth && o.CalYear == calYear && o.MetricID == metricId
                       select new KPIClass
                       {
                           SetId = o.SetID,
                           MetricId = o.MetricID,
                           BU = o.BU,
                           LOB = o.LOB,
                           CalYear = o.CalYear,
                           CalMonth = o.CalMonth,
                           FinYear = o.FinYear,
                           KPI1 = o.KPI1,
                           KPI2 = o.KPI2,
                           KPI3 = o.KPI3,
                           KPI4 = o.KPI4,
                           BGT = db.KPI_Agencies.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           ACT = db.KPI_Agencies.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF1 = db.KPI_Agencies.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF2 = db.KPI_Agencies.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF3 = db.KPI_Agencies.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           //UpdatedBy = o.UpdatedBy,
                           //UpdatedDt = o.UpdatedDt
                       }).Distinct().AsEnumerable();

            if (location != "All" && Session["MetricName"].ToString() != "Cargolux")
            {
                kpi = kpi.Where(a => location.Contains(a.KPI3)).Select(a => a).AsEnumerable();
            }

            kpi = kpi.Select(o => new KPIClass
            {
                SetId = o.SetId,
                MetricId = o.MetricId,
                BU = o.BU,
                LOB = o.LOB,
                CalYear = o.CalYear,
                CalMonth = o.CalMonth,
                CalMonth2 = DateTime.Parse(o.CalYear + "-" + o.CalMonth + "-01"),
                FinYear = o.FinYear,
                Currency = o.Currency,
                KPI1 = o.KPI1,
                KPI2 = o.KPI2,
                KPI3 = o.KPI3,
                KPI4 = o.KPI4,
                BGT = db.KPI_Agencies.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                ACT = db.KPI_Agencies.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF1 = db.KPI_Agencies.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF2 = db.KPI_Agencies.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF3 = db.KPI_Agencies.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                //UpdatedBy = o.UpdatedBy,
                //UpdatedDt = o.UpdatedDt
            }).Distinct().ToList();

            return Json(kpi, JsonRequestBehavior.AllowGet);
        }

        public bool EditExisting(string model, bool isRFZeroOnly)
        {
            string location = Session["Location"].ToString();
            var systemDate = ConfigurationManager.AppSettings["DebugSystemDate"];
            var systemMonth = "";
            if (systemDate != "")
            {
                systemMonth = systemDate.Split('-')[1];
            }
            else
            {
                systemMonth = DateTime.Now.ToString("MMM");
                systemDate = DateTime.Now.ToString("dd-MMM-yyyy");
            }

            string currentFiscalYear = helper.getCurrentFiscalYear();
            DateTime sysDate = DateTime.Parse(systemDate);

            var controlPeriod = (from c in db.KPI_ControlPeriods
                                 where c.FinYear == currentFiscalYear && c.Model == model
                                 select c).ToList();

            DateTime recStartDate = controlPeriod.FirstOrDefault().RecordStartDate;
            DateTime recEndDate = controlPeriod.FirstOrDefault().RecordEndDate;

            string LOB = Session["LF"].ToString();
            string metricName = Session["MetricName"].ToString();
            int metricId = db.KPI_Metrics.Where(m => m.MetricName == metricName).Select(m => m.ID).FirstOrDefault();

            var records = (from r in db.KPI_Agencies
                           where r.Model == model && r.FinYear == currentFiscalYear && r.MetricID == metricId
                           select r).ToList();
            if (location != "All" && Session["MetricName"].ToString() != "Cargolux")
            {
                records = records.Where(a => a.KPI3 == location).Select(a => a).ToList();
            }

            foreach (var item in records)
            {
                KPI_Agency rec = new KPI_Agency();
                rec = item;

                if (int.Parse(recStartDate.ToString("yyyy") + recStartDate.ToString("MM")) <= int.Parse(item.CalYear + DateTime.ParseExact(item.CalMonth, "MMM", CultureInfo.CurrentCulture).Month.ToString("00")) && int.Parse(recEndDate.ToString("yyyy") + recEndDate.ToString("MM")) >= int.Parse(item.CalYear + DateTime.ParseExact(item.CalMonth, "MMM", CultureInfo.CurrentCulture).Month.ToString("00")))
                {
                    switch (model)
                    {
                        case "RF1":
                            //replace value of RF1 with the value of BGT
                            var bgt = (from b in db.KPI_Agencies
                                       where b.CalMonth == item.CalMonth && b.CalYear == item.CalYear && (b.Model == "BGT" || b.Model == "RF1") && b.KPI1 == item.KPI1 && b.KPI2 == item.KPI2 && b.KPI3 == item.KPI3 && b.KPI4 == item.KPI4 && b.LOB == item.LOB && b.MetricID == item.MetricID && b.BU == item.BU && b.SetID == item.SetID
                                       select new { b.Model, b.Measure1 }).ToList();

                            if (bgt.Count > 0)
                            {
                                var measure = bgt.Where(r => r.Model == "RF1").Select(r => r.Measure1).FirstOrDefault();
                                if (isRFZeroOnly == true)
                                {
                                    if (measure == 0)
                                    {
                                        rec.Measure1 = bgt.Where(r => r.Model == "BGT").Select(r => r.Measure1).FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    rec.Measure1 = bgt.Where(r => r.Model == "BGT").Select(r => r.Measure1).FirstOrDefault();
                                }
                            }
                            else
                            {
                                rec.Measure1 = null;
                            }
                            break;

                        case "RF2":
                            //replace value of RF2 with the value of RF1
                            var rf1 = (from b in db.KPI_Agencies
                                       where b.CalMonth == item.CalMonth && b.CalYear == item.CalYear && (b.Model == "RF1" || b.Model == "RF2") && b.KPI1 == item.KPI1 && b.KPI2 == item.KPI2 && b.KPI3 == item.KPI3 && b.KPI4 == item.KPI4 && b.LOB == item.LOB && b.MetricID == item.MetricID && b.BU == item.BU && b.SetID == item.SetID
                                       select new { b.Model, b.Measure1 }).ToList();

                            if (rf1.Count > 0)
                            {
                                var measure = rf1.Where(r => r.Model == "RF2").Select(r => r.Measure1).FirstOrDefault();
                                if (isRFZeroOnly == true)
                                {
                                    if (measure == 0)
                                    {
                                        rec.Measure1 = rf1.Where(r => r.Model == "RF1").Select(r => r.Measure1).FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    rec.Measure1 = rf1.Where(r => r.Model == "RF1").Select(r => r.Measure1).FirstOrDefault();
                                }
                            }
                            else
                            {
                                rec.Measure1 = null;
                            }
                            break;
                        case "RF3":
                            //replace value of RF3 with the value of RF2
                            var rf2 = (from b in db.KPI_Agencies
                                       where b.CalMonth == item.CalMonth && b.CalYear == item.CalYear && (b.Model == "RF2" || b.Model == "RF3") && b.KPI1 == item.KPI1 && b.KPI2 == item.KPI2 && b.KPI3 == item.KPI3 && b.KPI4 == item.KPI4 && b.LOB == item.LOB && b.MetricID == item.MetricID && b.BU == item.BU && b.SetID == item.SetID
                                       select new { b.Model, b.Measure1 }).ToList();

                            if (rf2.Count > 0)
                            {
                                var measure = rf2.Where(r => r.Model == "RF3").Select(r => r.Measure1).FirstOrDefault();
                                if (isRFZeroOnly == true)
                                {
                                    if (measure == 0)
                                    {
                                        rec.Measure1 = rf2.Where(r => r.Model == "RF2").Select(r => r.Measure1).FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    rec.Measure1 = rf2.Where(r => r.Model == "RF2").Select(r => r.Measure1).FirstOrDefault();
                                }
                            }
                            else
                            {
                                rec.Measure1 = null;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var act = (from b in db.KPI_Agencies
                               where b.CalMonth == item.CalMonth && b.CalYear == item.CalYear && (b.Model == "ACT" || b.Model == model) && b.KPI1 == item.KPI1 && b.KPI2 == item.KPI2 && b.KPI3 == item.KPI3 && b.KPI4 == item.KPI4 && b.LOB == item.LOB && b.MetricID == item.MetricID && b.BU == item.BU && b.SetID == item.SetID
                               select new { b.Model, b.Measure1 }).ToList();

                    if (act.Count > 0)
                    {
                        var measure = act.Where(r => r.Model == model).Select(r => r.Measure1).FirstOrDefault();
                        if (isRFZeroOnly == true)
                        {
                            if (measure == 0)
                            {
                                rec.Measure1 = act.Where(r => r.Model == "ACT").Select(r => r.Measure1).FirstOrDefault();
                            }
                        }
                        else
                        {
                            rec.Measure1 = act.Where(r => r.Model == "ACT").Select(r => r.Measure1).FirstOrDefault();
                        }
                    }
                    else
                    {
                        rec.Measure1 = null;
                    }
                }

                if (rec.Measure1 != null)
                {
                    rec.UpdatedBy = Session["tkusername"].ToString();
                    rec.UpdatedDt = DateTime.Now;

                    db.KPI_Agencies.Attach(rec);

                    db.Entry(rec).Property(u => u.Measure1).IsModified = true;
                    db.Entry(rec).Property(u => u.UpdatedBy).IsModified = true;
                    db.Entry(rec).Property(u => u.UpdatedDt).IsModified = true;


                    db.SaveChanges();
                    LogManager.Log.Info("Measure1 for Id " + item.ID + " successfully updated.");
                }
            }

            return true;
        }

        public JsonResult CheckEditAll()
        {
            List<string> rfList = new List<string>();
            rfList = helper.checkEditAll();

            return Json(rfList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public bool IsEditable(string model, string finYear, string calMonth, string calYear)
        {
            bool isEditable = false;
            isEditable = helper.isEditable(model, finYear, calMonth, calYear);

            return isEditable;
        }

        public string GetCurrentFiscalYear()
        {
            string currentFinYear = "";
            currentFinYear = helper.getCurrentFiscalYear();

            return currentFinYear;
        }
    }
}