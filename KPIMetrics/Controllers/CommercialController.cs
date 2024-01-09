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
    public class CommercialController : Controller
    {
        string userName = "";
        string BU = "";
        Models.KPIMetrics db = new Models.KPIMetrics();
        KPIHelper helper = new KPIHelper();

        // GET: Commercial
        [SsoAuth]
        public ActionResult Index()
        {
            try
            {
                userName = Session["tkusername"].ToString();
                BU = db.KPI_BUs.Where(b => b.ID == 1).Select(b => b.Name).FirstOrDefault();
                            
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
                    string[] locs = user.Where(l=> l.Initial == userName && (l.BUId == 0 || l.BUId == 1)).Select(b => b.Location).ToArray();
                    LogManager.Log.Info("Redirecting to Index view...");

                    Session["Location"] = string.Join(",", locs);
                    Session["Role"] = string.Join(",", buids);

                    ViewBag.LF = Session["LF"];
                    ViewBag.KPI = Session["MetricName"];
                    string metricName = Session["MetricName"].ToString();

                    var kpis = (from k in db.KPI_Metrics
                                where k.MetricName == metricName
                                select k).ToList();                  

                    KPIClass kpi = new KPIClass();
                    kpi.ModelList = GetReference(0, "Model", "");
                    kpi.KPI1List = GetKPI1("");
                    kpi.KPI2List = GetKPI2("");
                    kpi.KPI3List = GetKPI3("");
                    kpi.KPI4List = GetReference(1, "Charter Type", "");
                    kpi.MonthList = GetMonth("");
                    kpi.LineFunctionList = helper.getLineFunction(BU, metricName);
                    //kpi.Currency = "USD";

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

                    ViewBag.LF = Session["LF"];
                    return View("Index", kpi);
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

        [SsoAuth]
        public ActionResult Management()
        {
            int metricId = db.KPI_Metrics.Where(l=> l.MetricName == "Management").Select(l=> l.ID).FirstOrDefault();
            string lineFunction = db.KPI_Line_Functions.Where(l=> l.BUId == 1 && l.MetricId == metricId).Select(l=> l.Name).FirstOrDefault();

            Session["LF"] = lineFunction;          
            Session["MetricName"] = "Management";
            return RedirectToAction("Index");
        }

        [SsoAuth]
        public ActionResult Advisory()
        {
            int metricId = db.KPI_Metrics.Where(l => l.MetricName == "Advisory").Select(l => l.ID).FirstOrDefault();
            string lineFunction = db.KPI_Line_Functions.Where(l => l.BUId == 1 && l.MetricId == metricId).Select(l => l.Name).FirstOrDefault();

            Session["LF"] = lineFunction;
            Session["MetricName"] = "Advisory";
            return RedirectToAction("Index");
        }

        public JsonResult List()
        {
            string LOB = "";
            LOB = Session["LF"].ToString();
            List<KPIClass> comm = new List<KPIClass>();
            var accounts = (from o in db.KPI_Commercials
                            where o.LOB == LOB
                            select new KPIClass
                            {
                                MetricId = o.MetricID,
                                SetId = o.SetID,
                                BU = o.BU,
                                LOB = o.LOB,
                                CalYear = o.CalYear,
                                CalMonth = o.CalMonth,
                                FinYear = o.FinYear,
                                Currency = o.Cur,
                                KPI1 = o.KPI1,
                                KPI2 = o.KPI2,
                                KPI3 = o.KPI3,
                                KPI4 = o.KPI4,
                                BGT = db.KPI_Commercials.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                ACT = db.KPI_Commercials.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                RF1 = db.KPI_Commercials.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                RF2 = db.KPI_Commercials.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                RF3 = db.KPI_Commercials.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                                //UpdatedBy = o.UpdatedBy,
                                //CreatedDt = o.CreatedDt
                            }).Distinct().AsEnumerable();

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
                Currency = o.Currency,
                KPI1 = o.KPI1,
                KPI2 = o.KPI2,
                KPI3 = o.KPI3,
                KPI4 = o.KPI4,
                BGT = db.KPI_Commercials.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                ACT = db.KPI_Commercials.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF1 = db.KPI_Commercials.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF2 = db.KPI_Commercials.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF3 = db.KPI_Commercials.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                //UpdatedBy = o.UpdatedBy,
                //CreatedDt = o.CreatedDt
            }).Distinct().ToList();

            foreach (var item in accounts)
            {
                KPIClass kpi = new KPIClass();
                kpi.Id = db.KPI_Commercials.Where(c => c.BU == item.BU && c.LOB == item.LOB && c.CalMonth == item.CalMonth && c.CalYear == item.CalYear && c.KPI1 == item.KPI1 && c.KPI2 == item.KPI2 && c.KPI3 == item.KPI3 && c.KPI4 == item.KPI4 && c.SetID == item.SetId).Select(c => c.ID).FirstOrDefault();
                kpi.SetId = item.SetId;
                kpi.MetricId = item.MetricId;
                kpi.BU = item.BU;
                kpi.LOB = item.LOB;
                kpi.CalYear = item.CalYear;
                kpi.CalMonth = item.CalMonth;
                kpi.CalMonth2 = item.CalMonth2;
                kpi.FinYear = item.FinYear;
                kpi.Currency = item.Currency;
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
                    int setId = db.KPI_Commercials.Count() == 0 ? 1 : db.KPI_Commercials.Max(c => c.SetID) + 1;
                    foreach (var model in models)
                    {
                        KPI_Commercial al = new KPI_Commercial();
                        al.SetID = setId;                  
                        al.MetricID = db.KPI_Metrics.Where(k => k.MetricName == metricName).Select(k => k.ID).FirstOrDefault();
                        al.BU = db.KPI_BUs.Where(b => b.ID == 1).Select(b => b.Name).FirstOrDefault();
                        al.LOB = Session["LF"].ToString();
                        al.CalYear = month <= 3 ? int.Parse(kpi.CalYear).ToString() : (int.Parse(kpi.CalYear) + 1).ToString();
                        al.CalMonth = GetMonthAbbr(month);
                        //al.FinYear = GetFinYear(month); //(dateNow.Year - finFactor1).ToString() + "-" + (dateNow.Year + finFactor2).ToString().Substring(2,2);
                        al.FinYear = kpi.CalYear + "-" + (int.Parse(kpi.CalYear) + 1).ToString().Substring(2, 2);
                        al.Cur = kpi.Currency;
                        al.Model = model;
                        al.KPI1 = kpi.KPI1;
                        al.KPI2 = kpi.KPI2;
                        al.KPI3 = kpi.KPI3;
                        al.KPI4 = kpi.KPI4;
                        al.Measure1 = 0;
                        al.Measure2 = (decimal?)null;
                        al.CreatedBy = Session["tkusername"].ToString();
                        al.CreatedDt = dateNow;

                        var commercial = (from c in db.KPI_Commercials
                                          where c.MetricID == al.MetricID && c.BU == al.BU && c.LOB == al.LOB && c.CalYear == al.CalYear && c.CalMonth == al.CalMonth && c.FinYear == al.FinYear && c.Model == model && c.KPI1 == al.KPI1 && c.KPI2 == al.KPI2 && c.KPI3 == al.KPI3 && c.KPI4 == al.KPI4 && c.SetID == al.SetID
                                          select c).ToList();

                        if (commercial.Count == 0)
                        {
                            db.KPI_Commercials.Add(al);
                            db.SaveChanges();
                            LogManager.Log.Info("Commercial Id " + al.ID + " was successfully added by " + Session["tkusername"].ToString() + ".");

                            KPI_metric_log log = new KPI_metric_log();
                            log.LogActionType = "New";
                            log.LogDatetime = DateTime.Now;
                            log.LogSource = "Add " + Session["LF"];
                            log.LogTableID = al.ID;
                            log.LogTablename = "KPI_Commercial";
                            log.LogUser = Session["tkusername"].ToString();                          

                            db.KPI_metric_logs.Add(log);
                            db.SaveChanges();                          

                            LogManager.Log.Info("Commercial Id " + al.ID + " was successfully added to the database log.");
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
                BU = db.KPI_BUs.Where(b => b.ID == 1).Select(b => b.Name).FirstOrDefault();
                var models = (from m in db.KPI_References
                              where m.RefType == "Model"
                              select m.RefValue).ToList();

                var ac = db.KPI_Commercials.Where(u => u.BU == BU && u.LOB == kpi.LOB && u.KPI1 == kpi.KPI1 && u.KPI2 == kpi.KPI2 && u.KPI3 == kpi.KPI3 && u.KPI4 == kpi.KPI4 && u.CalMonth == kpi.CalMonth && u.CalYear == kpi.CalYear && u.SetID == kpi.SetId).Select(u => u).ToList();
                KPI_Commercial al = new KPI_Commercial();


                LogManager.Log.Info("AC count: " + ac.Count());

                if (ac.ToList().Count() > 0)
                {
                    foreach (var model in models)
                    {
                        al = ac.Where(m => m.Model == model).FirstOrDefault();

                        var commercial = (from c in db.KPI_Commercials
                                          where c.MetricID == al.MetricID && c.BU == al.BU && c.LOB == al.LOB && c.CalYear == al.CalYear && c.CalMonth == al.CalMonth && c.FinYear == al.FinYear && c.Model == model && c.KPI1 == al.KPI1 && c.KPI2 == al.KPI2 && c.KPI3 == al.KPI3 && c.KPI4 == al.KPI4 && c.SetID == al.SetID
                                          select c).ToList();

                        LogManager.Log.Info("commercial count: " + commercial.Count());

                        if (commercial.Count == 0 && model != "")
                        {
                            string metricName = Session["MetricName"].ToString();

                            KPI_Commercial k = new KPI_Commercial();
                            k.MetricID = db.KPI_Metrics.Where(m => m.MetricName == metricName).Select(m => k.ID).FirstOrDefault();
                            k.SetID = al.SetID;
                            k.BU = db.KPI_BUs.Where(b => b.ID == 1).Select(b => b.Name).FirstOrDefault();
                            k.LOB = Session["LF"].ToString();
                            k.CalYear = al.CalYear;
                            k.CalMonth = al.CalMonth;                           
                            //k.FinYear = GetFinYear(item.CalMonth); //(dateNow.Year - finFactor1).ToString() + "-" + (dateNow.Year + finFactor2).ToString().Substring(2,2);
                            k.FinYear = al.FinYear;
                            k.Cur = al.Cur;
                            k.Model = model;
                            k.KPI1 = al.KPI1;
                            k.KPI2 = al.KPI2;
                            k.KPI3 = al.KPI3;
                            k.KPI4 = al.KPI4;
                            k.Measure1 = model == "ACT" ? kpi.ACT : (model == "RF1" ? kpi.RF1 : (model == "RF2" ? kpi.RF2 : (model == "BGT" ? kpi.BGT : kpi.RF3)));
                            k.Measure2 = (decimal?)null;
                            k.CreatedBy = Session["tkusername"].ToString();
                            k.CreatedDt = dateNow;


                            db.KPI_Commercials.Add(k);
                            db.SaveChanges();
                            LogManager.Log.Info("Commercial Id " + al.ID + " successfully added.");
                        }
                        else
                        {
                            string oldValue = al.Measure1.ToString();
                            al.Measure1 = model == "ACT" ? kpi.ACT : (model == "RF1" ? kpi.RF1 : (model == "RF2" ? kpi.RF2 : (model == "BGT" ? kpi.BGT : kpi.RF3)));
                            al.UpdatedBy = Session["tkusername"].ToString();
                            al.UpdatedDt = dateNow;

                            db.KPI_Commercials.Attach(al);

                            db.Entry(al).Property(u => u.Measure1).IsModified = true;
                            db.Entry(al).Property(u => u.UpdatedBy).IsModified = true;
                            db.Entry(al).Property(u => u.UpdatedDt).IsModified = true;

                            db.SaveChanges();
                            LogManager.Log.Info("Measure1 for Commercial Id " + al.ID + " was successfully updated by " + Session["tkusername"].ToString() + ".");

                            

                            if (al.Measure1 != null && oldValue != al.Measure1.Value.ToString("0.0000"))
                            {
                                KPI_metric_log log = new KPI_metric_log();
                                log.LogActionType = "Edit";
                                log.LogDatetime = DateTime.Now;
                                log.LogSource = "Update " + Session["LF"];
                                log.LogTableID = kpi.Id;
                                log.LogTablename = "KPI_Commercial";
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

                                LogManager.Log.Info("Commercial Id " + al.ID + " successfully added to the database log.");
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
            string kpi1 = db.KPI_Metrics.Where(k => k.BUID == 1 && k.MetricName == metricName).Select(k => k.KPI1).FirstOrDefault();

            var shipType = db.KPI_References.Where(s => s.RefType == kpi1).Select(s => s.RefValue);

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

        private SelectList GetKPI2(string SelectedValue)
        {
            List<string> customerList = new List<string>();
            var model = db.KPI_Commercials.Select(s => s.KPI2).Distinct();

            foreach (var a in model)
            {
                customerList.Add(a);
            }

            return new SelectList(customerList.ToList(), SelectedValue);
        }

        private SelectList GetKPI3(string SelectedValue)
        {
            List<string> shipList = new List<string>();
            var model = db.KPI_Commercials.Select(s => s.KPI3).Distinct();

            foreach (var a in model)
            {
                shipList.Add(a);
            }

            return new SelectList(shipList.ToList(), SelectedValue);
        }

        private SelectList GetMonth(string SelectedValue)
        {
            List<SelectListItem> monthList = new List<SelectListItem>();
            monthList = helper.getMonth(SelectedValue);

            return new SelectList(monthList.ToList(), "Value", "Text", SelectedValue);
        }

        //private SelectList GetLineFunction(string BU, string metricName)
        //{
        //    List<SelectListItem> monthList = new List<SelectListItem>();
        //    monthList = helper.getLineFunction(BU, metricName);

        //    return new SelectList(monthList.ToList(), "Value", "Text", 0);
        //}

        private string GetMonthAbbr(int month)
        {
            string monthAbbr = helper.getMonthAbbr(month);
            return monthAbbr;
        }

        private int GetMonthNum(string month)
        {
            int monthNo = 0;
            month = month.ToUpper();
            DateTime dt = DateTime.Parse("2022-" + ConfigurationManager.AppSettings["FirstMonthOfFY"].ToUpper() + "-01");

            for (int i = 1; i <= 12; i++)
            {
                if(dt.ToString("MMM").ToUpper() == month)
                {
                    monthNo = i;
                    break;
                }

                dt = dt.AddMonths(1);
            }



            //switch (month)
            //{
            //    case "OCT":
            //        monthNo = 1;
            //        break;

            //    case "NOV":
            //        monthNo = 2;
            //        break;

            //    case "DEC":
            //        monthNo = 3;
            //        break;

            //    case "JAN":
            //        monthNo = 4;
            //        break;

            //    case "FEB":
            //        monthNo = 5;
            //        break;

            //    case "MAR":
            //        monthNo = 6;
            //        break;

            //    case "APR":
            //        monthNo = 7;
            //        break;

            //    case "MAY":
            //        monthNo = 8;
            //        break;

            //    case "JUN":
            //        monthNo = 9;
            //        break;

            //    case "JUL":
            //        monthNo = 10;
            //        break;

            //    case "AUG":
            //        monthNo = 11;
            //        break;

            //    case "SEP":
            //        monthNo = 12;
            //        break;

            //    default:
            //        break;
            //}

            return monthNo;
        }

        //private string GetFinYear(int month)
        //{
        //    string finYear = "";
        //    string[] monthAbbrev = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;
        //    int monthNum = Array.IndexOf(monthAbbrev, month) + 1;

        //    int finFactor1 = month >= 10 ? 0 : -1;
        //    int finFactor2 = monthNum >= 10 ? +1 : 0;
        //    int calMonth = DateTime.Now.Month;

        //    if (calMonth >= 10)
        //    {
        //        finYear = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Year + 1).ToString().Substring(2, 2);
        //    }
        //    else
        //    {
        //        if (monthNum >= 10)
        //        {
        //            //current year - (current year + 1)
        //            finYear = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Year + 1).ToString().Substring(2, 2);
        //        }
        //        else
        //        {
        //            finYear = (DateTime.Now.Year - 1).ToString() + "-" + (DateTime.Now.Year).ToString().Substring(2, 2);
        //        }
        //    }

        //    return finYear; ;
        //}

        public JsonResult GetCustomerRec(string customer, string shipname, string calMonth, string calYear)
        {
            string LOB = Session["LF"].ToString();
            if (LOB != "Commercial Management")
            {
                shipname = null;
            }

            var kpi = (from o in db.KPI_Commercials
                       where o.KPI2 == customer && o.LOB == LOB
                       select new KPIClass
                       {
                           MetricId = o.MetricID,
                           SetId = o.SetID,
                           BU = o.BU,
                           LOB = o.LOB,
                           CalYear = o.CalYear,
                           CalMonth = o.CalMonth,
                           FinYear = o.FinYear,
                           Currency = o.Cur,
                           KPI1 = o.KPI1,
                           KPI2 = o.KPI2,
                           KPI3 = o.KPI3,
                           KPI4 = o.KPI4,
                           BGT = db.KPI_Commercials.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           ACT = db.KPI_Commercials.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF1 = db.KPI_Commercials.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF2 = db.KPI_Commercials.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF3 = db.KPI_Commercials.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           //UpdatedBy = o.UpdatedBy,
                           //UpdatedDt = o.UpdatedDt
                       }).Distinct().AsEnumerable();

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
                BGT = db.KPI_Commercials.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                ACT = db.KPI_Commercials.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF1 = db.KPI_Commercials.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF2 = db.KPI_Commercials.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF3 = db.KPI_Commercials.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                //UpdatedBy = o.UpdatedBy,
                //UpdatedDt = o.UpdatedDt
            }).Distinct().ToList();

            return Json(kpi, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonthYearRec(string customer, string shipname, string calMonth, string calYear)
        {
            string LOB = Session["LF"].ToString();
            var kpi = (from o in db.KPI_Commercials
                       where o.CalMonth == calMonth && o.CalYear == calYear && o.LOB == LOB
                       select new KPIClass
                       {
                           SetId = o.SetID,
                           MetricId = o.MetricID,
                           BU = o.BU,
                           LOB = o.LOB,
                           CalYear = o.CalYear,
                           CalMonth = o.CalMonth,
                           FinYear = o.FinYear,
                           Currency = o.Cur,
                           KPI1 = o.KPI1,
                           KPI2 = o.KPI2,
                           KPI3 = o.KPI3,
                           KPI4 = o.KPI4,
                           BGT = db.KPI_Commercials.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           ACT = db.KPI_Commercials.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF1 = db.KPI_Commercials.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF2 = db.KPI_Commercials.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           RF3 = db.KPI_Commercials.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetID).Select(c => c.Measure1).Sum(),
                           //UpdatedBy = o.UpdatedBy,
                           //UpdatedDt = o.UpdatedDt
                       }).Distinct().AsEnumerable();

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
                BGT = db.KPI_Commercials.Where(c => c.Model == "BGT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                ACT = db.KPI_Commercials.Where(c => c.Model == "ACT" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF1 = db.KPI_Commercials.Where(c => c.Model == "RF1" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF2 = db.KPI_Commercials.Where(c => c.Model == "RF2" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                RF3 = db.KPI_Commercials.Where(c => c.Model == "RF3" && c.BU == o.BU && c.LOB == o.LOB && c.CalMonth == o.CalMonth && c.CalYear == o.CalYear && c.FinYear == o.FinYear && c.KPI1 == o.KPI1 && c.KPI2 == o.KPI2 && c.KPI3 == o.KPI3 && c.KPI4 == o.KPI4 && c.SetID == o.SetId).Select(c => c.Measure1).Sum(),
                //UpdatedBy = o.UpdatedBy,
                //UpdatedDt = o.UpdatedDt
            }).Distinct().ToList();

            return Json(kpi, JsonRequestBehavior.AllowGet);
        }

        public bool EditExisting(string model, bool isRFZeroOnly)
        {
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

            var records = (from r in db.KPI_Commercials
                           where r.Model == model && r.FinYear == currentFiscalYear && r.LOB == LOB
                           select r).ToList();

            foreach (var item in records)
            {
                KPI_Commercial rec = new KPI_Commercial();
                rec = item;

                if (int.Parse(recStartDate.ToString("yyyy") + recStartDate.ToString("MM")) <= int.Parse(item.CalYear + DateTime.ParseExact(item.CalMonth, "MMM", CultureInfo.CurrentCulture).Month.ToString("00")) && int.Parse(recEndDate.ToString("yyyy") + recEndDate.ToString("MM")) >= int.Parse(item.CalYear + DateTime.ParseExact(item.CalMonth, "MMM", CultureInfo.CurrentCulture).Month.ToString("00")))
                {
                    switch (model)
                    {
                        case "RF1":
                            //replace value of RF1 with the value of BGT
                            var bgt = (from b in db.KPI_Commercials
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
                            var rf1 = (from b in db.KPI_Commercials
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
                            var rf2 = (from b in db.KPI_Commercials
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
                    var act = (from b in db.KPI_Commercials
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
                    rec.UpdatedBy = Session["tkusername"].ToString(); ;
                    rec.UpdatedDt = DateTime.Now;

                    db.KPI_Commercials.Attach(rec);

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

        public JsonResult CustomerList()
        {
            var customer = db.KPI_Commercials.Select(c => c.KPI2).Distinct().OrderBy(c => c);

            return Json(customer, JsonRequestBehavior.AllowGet);
        }
    }
}