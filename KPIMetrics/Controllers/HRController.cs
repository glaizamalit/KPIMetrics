using KPIMetrics.Models;
using KPIMetrics.Sso;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KPIMetrics.Controllers
{
    public class HRController : Controller
    {
        string userName = "";
        Models.KPIMetrics db = new Models.KPIMetrics();
        KPIHelper helper = new KPIHelper();
        string firstMonth, secondMonth, thirdMonth, fourthMonth, fifthMonth, sixthMonth, seventhMonth, eighthMonth, ninthMonth, tenthMonth, eleventhMonth, twelfthMonth;
        string firstYear, secondYear, thirdYear, fourthYear, fifthYear, sixthYear, seventhYear, eighthYear, ninthYear, tenthYear, eleventhYear, twelfthYear;
        DateTime? dateFY = null;
        // GET: HR
        [SsoAuth]
        public ActionResult List()
        {
            try
            {
                userName = Session["tkusername"].ToString();
                getMonthsAndYear();
                Session["SystemDate"] = ConfigurationManager.AppSettings["DebugSystemDate"].ToString() == "" ? DateTime.Now.ToString("dd-MMM-yyyy") : ConfigurationManager.AppSettings["DebugSystemDate"].ToString();


                UserInfo userRec = new UserInfo();
                var user = (from u in db.Users
                            join ul in db.UserLocations on u.Initials equals ul.Initials
                            join bl in db.BULocations on ul.BULocID equals bl.ID
                            where u.Initials == userName
                            select new UserInfo
                            {
                                Initial = u.Initials,
                                BUId = bl.BUID,
                                LocationId = ul.ID,
                                Location = bl.Location
                            }).ToList();
                if (user.Count() > 0)
                {
                    userRec = user.FirstOrDefault();
                }

                if (userRec.Initial != null)
                {
                    int[] buids = user.Select(b => b.BUId).ToArray();
                    string[] locs = user.Where(l => l.Initial == userName && (l.BUId == 0 || l.BUId == 4)).Select(b => b.Location).ToArray();
                    LogManager.Log.Info("Redirecting to Index view...");

                    Session["Role"] = string.Join(",", buids);

                    StaffClass staffInfo = new StaffClass();
                    staffInfo.MYTitle1 = firstMonth + "-" + firstYear.Substring(2, 2);
                    staffInfo.MYTitle2 = secondMonth + "-" + secondYear.Substring(2, 2);
                    staffInfo.MYTitle3 = thirdMonth + "-" + thirdYear.Substring(2, 2);
                    staffInfo.MYTitle4 = fourthMonth + "-" + fourthYear.Substring(2, 2);
                    staffInfo.MYTitle5 = fifthMonth + "-" + fifthYear.Substring(2, 2);
                    staffInfo.MYTitle6 = sixthMonth + "-" + sixthYear.Substring(2, 2);
                    staffInfo.MYTitle7 = seventhMonth + "-" + seventhYear.Substring(2, 2);
                    staffInfo.MYTitle8 = eighthMonth + "-" + eighthYear.Substring(2, 2);
                    staffInfo.MYTitle9 = ninthMonth + "-" + ninthYear.Substring(2, 2);
                    staffInfo.MYTitle10 = tenthMonth + "-" + tenthYear.Substring(2, 2);
                    staffInfo.MYTitle11 = eleventhMonth + "-" + eleventhYear.Substring(2, 2);
                    staffInfo.MYTitle12 = twelfthMonth + "-" + twelfthYear.Substring(2, 2);
                    return View("Index", staffInfo);
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

        public JsonResult Update(StaffClass staff)
        {
            try
            {
                var st = db.StaffList_Fins.Where(s => s.Emp_No == staff.emp_no).Select(s => s).ToList();
                if (st.Count == 0)
                {
                    StaffList_Fin em = new StaffList_Fin();
                    em.Emp_No = staff.emp_no;
                    em.AXcode = staff.AXcode;
                    em.BU_Fin = staff.BU_Fin;
                    em.Division_Fin = staff.Division_Fin != "" && staff.Division_Fin != null ? staff.Division_Fin.Trim() : staff.Division_Fin;
                    em.Entity_Fin = staff.Entity_Fin != "" && staff.Entity_Fin != null ? staff.Entity_Fin.Trim() : staff.Entity_Fin;
                    em.Func_Fin = staff.Func_Fin != "" && staff.Func_Fin != null ? staff.Func_Fin.Trim() : staff.Func_Fin;
                    em.Job_Fin = staff.Job_Fin != "" && staff.Job_Fin != null ? staff.Job_Fin.Trim() : staff.Job_Fin;
                    em.StaffType = staff.StaffType != "" && staff.StaffType != null ? staff.StaffType.Trim() : staff.StaffType;
                    em.Headcount = staff.HeadcountText == "Yes" ? true : (staff.HeadcountText == "No" ? false : (bool?)null);
                    em.HeadcountRemarks = staff.HeadcountRemarks;
                    em.CreatedBy = Session["tkusername"].ToString();
                    em.CreatedDT = DateTime.Now;

                    db.StaffList_Fins.Add(em);
                    db.SaveChanges();
                    LogManager.Log.Info("Employee Id " + staff.emp_no + " in StaffList_Fin table was successfully added by " + Session["tkusername"].ToString() + ".");
                }
                else
                {
                    var emp = db.StaffList_Fins.Where(s => s.Emp_No == staff.emp_no).Select(s => s).ToList();
                    StaffList_Fin em = new StaffList_Fin();
                    em = emp.FirstOrDefault();
                    em.AXcode = staff.AXcode;
                    em.BU_Fin = staff.BU_Fin;
                    em.Division_Fin = staff.Division_Fin;
                    em.Entity_Fin = staff.Entity_Fin;
                    em.Func_Fin = staff.Func_Fin;
                    em.Job_Fin = staff.Job_Fin;
                    em.StaffType = staff.StaffType;
                    em.Headcount = staff.HeadcountText == "Yes" ? true : (staff.HeadcountText == "No" ? false : (bool?)null);
                    em.HeadcountRemarks = staff.HeadcountRemarks;
                    em.UpdatedBy = Session["tkusername"].ToString();
                    em.UpdatedDT = DateTime.Now;

                    db.StaffList_Fins.Attach(em);

                    db.Entry(em).Property(u => u.AXcode).IsModified = true;
                    db.Entry(em).Property(u => u.BU_Fin).IsModified = true;
                    db.Entry(em).Property(u => u.Division_Fin).IsModified = true;
                    db.Entry(em).Property(u => u.Entity_Fin).IsModified = true;
                    db.Entry(em).Property(u => u.Func_Fin).IsModified = true;
                    db.Entry(em).Property(u => u.Job_Fin).IsModified = true;
                    db.Entry(em).Property(u => u.StaffType).IsModified = true;
                    db.Entry(em).Property(u => u.Headcount).IsModified = true;
                    db.Entry(em).Property(u => u.HeadcountRemarks).IsModified = true;
                    db.Entry(em).Property(u => u.UpdatedBy).IsModified = true;
                    db.Entry(em).Property(u => u.UpdatedDT).IsModified = true;

                    db.SaveChanges();
                    LogManager.Log.Info("Employee id " + staff.emp_no + " in StaffList_Fin table was successfully updated by " + Session["tkusername"].ToString() + ".");
                }

                getMonthsAndYear();
                string month = "";
                string year = "";
                bool? headcount = false;

                for (int i = 1; i <= 12; i++)
                {
                    month = dateFY.Value.ToString("MMM");
                    year = dateFY.Value.ToString("yyyy");
                    var hc = db.StaffHeadcounts.Where(s => s.Emp_No == staff.emp_no && s.Month == month && s.Year == year).Select(s => s).ToList();

                    switch (i)
                    {
                        case 1:
                            headcount = staff.MonthYearHC1 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 2:
                            headcount = staff.MonthYearHC2 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 3:
                            headcount = staff.MonthYearHC3 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 4:
                            headcount = staff.MonthYearHC4 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 5:
                            headcount = staff.MonthYearHC5 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 6:
                            headcount = staff.MonthYearHC6 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 7:
                            headcount = staff.MonthYearHC7 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 8:
                            headcount = staff.MonthYearHC8 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 9:
                            headcount = staff.MonthYearHC9 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 10:
                            headcount = staff.MonthYearHC10 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 11:
                            headcount = staff.MonthYearHC11 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        case 12:
                            headcount = staff.MonthYearHC12 == "Yes" ? true : (staff.MonthYearHC1 == "No" ? false : (bool?)null);
                            break;
                        default:
                            break;
                    }


                    if (hc.Count == 0)
                    {
                        StaffHeadcount em = new StaffHeadcount();
                        em.Emp_No = staff.emp_no;
                        em.Month = month;
                        em.Year = year;
                        em.Headcount = headcount;
                        em.CreatedBy = Session["tkusername"].ToString();
                        em.CreatedDt = DateTime.Now;

                        db.StaffHeadcounts.Add(em);
                        db.SaveChanges();
                        LogManager.Log.Info("Employee Id " + staff.emp_no + " in StaffHeadcount table was successfully added by " + Session["tkusername"].ToString() + ".");
                    }
                    else
                    {
                        StaffHeadcount em = new StaffHeadcount();
                        em = hc.FirstOrDefault();
                        em.Headcount = headcount;
                        em.UpdatedBy = Session["tkusername"].ToString();
                        em.UpdatedDt = DateTime.Now;

                        db.StaffHeadcounts.Attach(em);

                        db.Entry(em).Property(u => u.Headcount).IsModified = true;
                        db.Entry(em).Property(u => u.UpdatedBy).IsModified = true;
                        db.Entry(em).Property(u => u.UpdatedDt).IsModified = true;

                        db.SaveChanges();
                        LogManager.Log.Info("Employee id " + staff.emp_no + " in StaffHeadcount table was successfully updated by " + Session["tkusername"].ToString() + ".");
                    }


                    dateFY = dateFY.Value.AddMonths(1);
                }



                return Json(staff, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult StaffList(bool? isShowAll)
        {
            try
            {
                userName = Session["tkusername"].ToString();
                getMonthsAndYear();

                DateTime dt = DateTime.Now.AddMonths(-3);
                var staffRec = (from u in db.StaffLists
                                    //orderby u.StaffType, u.Division_Fin, u.BU_Fin, u.Func_Fin, u.Job_Fin, u.AXcode, u.Entity_Fin, u.Wallem_Initial
                                    //where u.emp_no == "W00310"
                                select new StaffClass()
                                {
                                    ID = 0,
                                    Initial = u.Wallem_Initial,
                                    emp_no = u.emp_no,
                                    DisplayName = u.DisplayName,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Title = u.Title,
                                    Dept_Desc = u.Dept_Desc,
                                    Division = u.Division,
                                    BranchCity = u.BranchCity,
                                    Country = u.Country,
                                    JobFamily = u.JobFamily,
                                    Emp_Entity = u.Emp_Entity,
                                    JobCode = u.JobCode,
                                    DateJoined = u.DateJoined,
                                    DateExited = u.DateExited,
                                    PrimaryEmail = u.PrimaryEmail,
                                    StaffType = u.StaffType,
                                    Division_Fin = u.Division_Fin,
                                    BU_Fin = u.BU_Fin,
                                    Func_Fin = u.Func_Fin,
                                    Job_Fin = u.Job_Fin,
                                    AXcode = u.AXcode,
                                    Entity_Fin = u.Entity_Fin,
                                    Headcount = u.Headcount,
                                    HeadcountText = u.Headcount == true ? "Yes" : (u.Headcount == false ? "No" : "For Updating"),
                                    IsDefault = (u.DateExited == null || (u.DateExited >= dt)) ? true : false,
                                    MYTitle1 = firstMonth + "-" + firstYear.Substring(2, 2),
                                    MYTitle2 = secondMonth + "-" + secondYear.Substring(2, 2),
                                    MYTitle3 = thirdMonth + "-" + thirdYear.Substring(2, 2),
                                    MYTitle4 = fourthMonth + "-" + fourthYear.Substring(2, 2),
                                    MYTitle5 = fifthMonth + "-" + fifthYear.Substring(2, 2),
                                    MYTitle6 = sixthMonth + "-" + sixthYear.Substring(2, 2),
                                    MYTitle7 = seventhMonth + "-" + seventhYear.Substring(2, 2),
                                    MYTitle8 = eighthMonth + "-" + eighthYear.Substring(2, 2),
                                    MYTitle9 = ninthMonth + "-" + ninthYear.Substring(2, 2),
                                    MYTitle10 = tenthMonth + "-" + tenthYear.Substring(2, 2),
                                    MYTitle11 = eleventhMonth + "-" + eleventhYear.Substring(2, 2),
                                    MYTitle12 = twelfthMonth + "-" + twelfthYear.Substring(2, 2),
                                    MonthYearHC1 = db.StaffHeadcounts.Where(c => c.Month == firstMonth && c.Year == firstYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == firstMonth && c.Year == firstYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC2 = db.StaffHeadcounts.Where(c => c.Month == secondMonth && c.Year == secondYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == secondMonth && c.Year == secondYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC3 = db.StaffHeadcounts.Where(c => c.Month == thirdMonth && c.Year == thirdYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == thirdMonth && c.Year == thirdYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC4 = db.StaffHeadcounts.Where(c => c.Month == fourthMonth && c.Year == fourthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == fourthMonth && c.Year == fourthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC5 = db.StaffHeadcounts.Where(c => c.Month == fifthMonth && c.Year == fifthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == fifthMonth && c.Year == fifthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC6 = db.StaffHeadcounts.Where(c => c.Month == sixthMonth && c.Year == sixthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == sixthMonth && c.Year == sixthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC7 = db.StaffHeadcounts.Where(c => c.Month == seventhMonth && c.Year == seventhYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == seventhMonth && c.Year == seventhYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC8 = db.StaffHeadcounts.Where(c => c.Month == eighthMonth && c.Year == eighthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == eighthMonth && c.Year == eighthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC9 = db.StaffHeadcounts.Where(c => c.Month == ninthMonth && c.Year == ninthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == ninthMonth && c.Year == ninthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC10 = db.StaffHeadcounts.Where(c => c.Month == tenthMonth && c.Year == tenthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == tenthMonth && c.Year == tenthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC11 = db.StaffHeadcounts.Where(c => c.Month == eleventhMonth && c.Year == eleventhYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == eleventhMonth && c.Year == eleventhYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                    MonthYearHC12 = db.StaffHeadcounts.Where(c => c.Month == twelfthMonth && c.Year == twelfthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == twelfthMonth && c.Year == twelfthYear && c.Emp_No == u.emp_no).Select(c => c.Headcount).FirstOrDefault() == true ? "Yes" : "No"),
                                })
                                 .Union(
                                    from v in db.StaffVacancies
                                    join s in db.StaffList_Fins on v.Emp_No equals s.Emp_No into st
                                    from p in st.DefaultIfEmpty()
                                    where v.IsDeleted == false
                                    select new StaffClass()
                                    {
                                        ID = v.ID,
                                        Initial = null,
                                        emp_no = v.Emp_No,
                                        DisplayName = "",
                                        FirstName = "",
                                        LastName = "",
                                        Title = v.Title,
                                        Dept_Desc = v.Dept_Desc,
                                        Division = v.Division,
                                        BranchCity = "",
                                        Country = "",
                                        JobFamily = "",
                                        Emp_Entity = "",
                                        JobCode = "",
                                        DateJoined = null,
                                        DateExited = null,
                                        PrimaryEmail = "",
                                        StaffType = p.StaffType,
                                        Division_Fin = p.Division_Fin,
                                        BU_Fin = p.BU_Fin,
                                        Func_Fin = p.Func_Fin,
                                        Job_Fin = p.Job_Fin,
                                        AXcode = p.AXcode,
                                        Entity_Fin = p.Entity_Fin,
                                        Headcount = db.StaffList_Fins.Where(s => s.Emp_No == v.Emp_No).Select(s => s.Headcount).FirstOrDefault(),
                                        HeadcountText = db.StaffList_Fins.Where(s => s.Emp_No == v.Emp_No).Select(s => s.Headcount).FirstOrDefault() == true ? "Yes" : (db.StaffList_Fins.Where(s => s.Emp_No == v.Emp_No).Select(s => s.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        IsDefault = true,
                                        MYTitle1 = firstMonth + "-" + firstYear.Substring(2, 2),
                                        MYTitle2 = secondMonth + "-" + secondYear.Substring(2, 2),
                                        MYTitle3 = thirdMonth + "-" + thirdYear.Substring(2, 2),
                                        MYTitle4 = fourthMonth + "-" + fourthYear.Substring(2, 2),
                                        MYTitle5 = fifthMonth + "-" + fifthYear.Substring(2, 2),
                                        MYTitle6 = sixthMonth + "-" + sixthYear.Substring(2, 2),
                                        MYTitle7 = seventhMonth + "-" + seventhYear.Substring(2, 2),
                                        MYTitle8 = eighthMonth + "-" + eighthYear.Substring(2, 2),
                                        MYTitle9 = ninthMonth + "-" + ninthYear.Substring(2, 2),
                                        MYTitle10 = tenthMonth + "-" + tenthYear.Substring(2, 2),
                                        MYTitle11 = eleventhMonth + "-" + eleventhYear.Substring(2, 2),
                                        MYTitle12 = twelfthMonth + "-" + twelfthYear.Substring(2, 2),
                                        MonthYearHC1 = db.StaffHeadcounts.Where(c => c.Month == firstMonth && c.Year == firstYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == firstMonth && c.Year == firstYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC2 = db.StaffHeadcounts.Where(c => c.Month == secondMonth && c.Year == secondYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == secondMonth && c.Year == secondYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC3 = db.StaffHeadcounts.Where(c => c.Month == thirdMonth && c.Year == thirdYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == thirdMonth && c.Year == thirdYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC4 = db.StaffHeadcounts.Where(c => c.Month == fourthMonth && c.Year == fourthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == fourthMonth && c.Year == fourthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC5 = db.StaffHeadcounts.Where(c => c.Month == fifthMonth && c.Year == fifthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == fifthMonth && c.Year == fifthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC6 = db.StaffHeadcounts.Where(c => c.Month == sixthMonth && c.Year == sixthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == sixthMonth && c.Year == sixthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC7 = db.StaffHeadcounts.Where(c => c.Month == seventhMonth && c.Year == seventhYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == seventhMonth && c.Year == seventhYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC8 = db.StaffHeadcounts.Where(c => c.Month == eighthMonth && c.Year == eighthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == eighthMonth && c.Year == eighthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC9 = db.StaffHeadcounts.Where(c => c.Month == ninthMonth && c.Year == ninthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == ninthMonth && c.Year == ninthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC10 = db.StaffHeadcounts.Where(c => c.Month == tenthMonth && c.Year == tenthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == tenthMonth && c.Year == tenthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC11 = db.StaffHeadcounts.Where(c => c.Month == eleventhMonth && c.Year == eleventhYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == eleventhMonth && c.Year == eleventhYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                        MonthYearHC12 = db.StaffHeadcounts.Where(c => c.Month == twelfthMonth && c.Year == twelfthYear && c.VacancyID == v.ID).Select(c => c.Headcount).Count() == 0 ? "For Updating" : (db.StaffHeadcounts.Where(c => c.Month == twelfthMonth && c.Year == twelfthYear && c.VacancyID == v.ID).Select(c => c.Headcount).FirstOrDefault() == false ? "No" : "For Updating"),
                                    }).ToList();

                var jsonResult = Json(staffRec, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult VacancyList()
        {
            try
            {
                var vacancyRec = (from v in db.StaffVacancies
                                  where v.IsDeleted == false
                                  select new StaffClass()
                                  {
                                      ID = v.ID,
                                      emp_no = v.Emp_No,
                                      Title = v.Title,
                                      Dept_Desc = v.Dept_Desc,
                                      Division = v.Division
                                  }).ToList();

                var jsonResult = Json(vacancyRec, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult CheckDuplication(string empno, string isNew)
        {
            bool isValid = false;
            if (isNew == "false")
            {
                isValid = true;
            }
            else
            {
                var staffList = db.StaffLists.Where(s => s.emp_no == empno).Select(s => s).ToList();
                if (staffList.Count == 0)
                {
                    isValid = true;
                }
            }
            return Json(isValid.ToString().ToUpper(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult VacancyAdd(StaffClass staff)
        {
            try
            {
                userName = Session["tkusername"].ToString();
                var staffList = db.StaffLists.Where(s => s.emp_no == staff.emp_no).Select(s => s).ToList();

                if (staffList.Count() == 0)
                {
                    StaffVacancy vacancy = new StaffVacancy();
                    vacancy.Emp_No = staff.emp_no;
                    vacancy.Title = staff.Title;
                    vacancy.Dept_Desc = staff.Dept_Desc;
                    vacancy.Division = staff.Division;
                    vacancy.IsDeleted = false;
                    vacancy.CreatedBy = userName;
                    vacancy.CreatedDt = DateTime.Now;
                    db.StaffVacancies.Add(vacancy);
                    db.SaveChanges();

                    getMonthsAndYear();
                    bool isValid = false;
                    //add record in headcount table
                    for (int i = 1; i <= 12; i++)
                    {
                        StaffHeadcount hc = new StaffHeadcount();
                        if ((DateTime.Now.Month.ToString("MMM") == dateFY.Value.Month.ToString("MMM") && DateTime.Now.Year.ToString("yyyy") == dateFY.Value.Year.ToString("yyyy")) || isValid)
                        {
                            hc.Emp_No = staff.emp_no;
                            hc.Year = dateFY.Value.Year.ToString();
                            hc.Month = dateFY.Value.ToString("MMM");
                            hc.Headcount = null;
                            hc.VacancyID = vacancy.ID;
                            hc.CreatedBy = userName;
                            hc.CreatedDt = DateTime.Now;
                            db.StaffHeadcounts.Add(hc);

                            isValid = true;

                            db.SaveChanges();
                            LogManager.Log.Info("Staff emp id " + hc.Emp_No + " in StaffHeadcount table was successfully added by " + Session["tkusername"].ToString() + ".");
                        }

                        dateFY = dateFY.Value.AddMonths(1);
                    }
                }

                return Json(staff, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult VacancyUpdate(StaffClass staff)
        {
            try
            {
                userName = Session["tkusername"].ToString();
                var staffLst = db.StaffLists.Where(s => s.emp_no == staff.emp_no).Select(s => s).ToList();
                int? vacId = null;
                bool isDeleted = false;
                if (staffLst.Count() > 0)
                {
                    vacId = null;
                    isDeleted = true;
                }
                else
                {
                    vacId = staff.ID;
                }

                var st = db.StaffVacancies.Where(s => s.ID == staff.ID).Select(s => s).FirstOrDefault();

                StaffVacancy em = new StaffVacancy();
                em = st;
                em.Emp_No = staff.emp_no;
                em.Title = staff.Title;
                em.Dept_Desc = staff.Dept_Desc;
                em.Division = staff.Division;
                em.IsDeleted = isDeleted;
                em.UpdatedBy = userName;
                em.UpdatedDt = DateTime.Now;
                db.StaffVacancies.Attach(em);

                db.Entry(em).Property(u => u.Emp_No).IsModified = true;
                db.Entry(em).Property(u => u.Title).IsModified = true;
                db.Entry(em).Property(u => u.Dept_Desc).IsModified = true;
                db.Entry(em).Property(u => u.Division).IsModified = true;
                db.Entry(em).Property(u => u.IsDeleted).IsModified = true;
                db.Entry(em).Property(u => u.UpdatedBy).IsModified = true;
                db.Entry(em).Property(u => u.UpdatedDt).IsModified = true;

                db.SaveChanges();
                LogManager.Log.Info("Staff vacancy id " + staff.emp_no + " in StaffVacancy table was successfully updated by " + Session["tkusername"].ToString() + ".");

                var hc = db.StaffHeadcounts.Where(ht => ht.Emp_No == staff.emp_no && ht.IsDeleted == false).Select(ht => ht).ToList();

                if (hc.Count() > 0)
                {
                    foreach (var item in hc)
                    {
                        StaffHeadcount h = new StaffHeadcount();

                        if (item.VacancyID == null)
                        {
                            h = item;
                            h.IsDeleted = true;
                            h.UpdatedBy = userName;
                            h.UpdatedDt = DateTime.Now;
                            db.StaffHeadcounts.Attach(h);
                            db.Entry(h).Property(u => u.IsDeleted).IsModified = true;
                            db.Entry(h).Property(u => u.UpdatedBy).IsModified = true;
                            db.Entry(h).Property(u => u.UpdatedBy).IsModified = true;

                            db.SaveChanges();
                            LogManager.Log.Info("Previous record for employee number " + staff.emp_no + " in StaffHeadcount table was successfully updated by " + Session["tkusername"].ToString() + ".");
                        }
                    }
                }

                var hcv = db.StaffHeadcounts.Where(ht => ht.VacancyID == staff.ID).Select(ht => ht).ToList();

                foreach (var item in hcv)
                {
                    StaffHeadcount h = new StaffHeadcount();
                    h = item;
                    h.Emp_No = staff.emp_no;
                    h.VacancyID = vacId;
                    h.UpdatedBy = userName;
                    h.UpdatedDt = DateTime.Now;
                    db.StaffHeadcounts.Attach(h);

                    db.Entry(h).Property(u => u.Emp_No).IsModified = true;
                    db.Entry(h).Property(u => u.VacancyID).IsModified = true;
                    db.Entry(h).Property(u => u.UpdatedBy).IsModified = true;
                    db.Entry(h).Property(u => u.UpdatedDt).IsModified = true;

                    db.SaveChanges();
                    LogManager.Log.Info("Staff vacancy id " + staff.emp_no + " in StaffVacancy and StaffHeadcount tables were successfully updated by " + Session["tkusername"].ToString() + ".");
                }

                return Json(staff, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult VacancyDelete(StaffClass staff)
        {
            try
            {
                userName = Session["tkusername"].ToString();

                var vc = db.StaffVacancies.Where(s => s.ID == staff.ID).Select(s => s).FirstOrDefault();
                StaffVacancy st = new StaffVacancy();
                st = vc;
                st.IsDeleted = true;
                db.StaffVacancies.Attach(vc);
                db.Entry(st).Property(u => u.IsDeleted).IsModified = true;
                db.SaveChanges();

                var hc = db.StaffHeadcounts.Where(h => h.Emp_No == staff.emp_no && h.IsDeleted == false && h.VacancyID == staff.ID).Select(h => h).ToList();
                foreach (var item in hc)
                {
                    StaffHeadcount ht = new StaffHeadcount();
                    ht = item;
                    ht.IsDeleted = true;
                    ht.UpdatedBy = userName;
                    ht.UpdatedDt = DateTime.Now;

                    db.StaffHeadcounts.Attach(ht);

                    db.Entry(ht).Property(u => u.IsDeleted).IsModified = true;
                    db.Entry(ht).Property(u => u.UpdatedBy).IsModified = true;
                    db.Entry(ht).Property(u => u.UpdatedDt).IsModified = true;

                    db.SaveChanges();
                }

                LogManager.Log.Info("Staff vacancy id " + staff.emp_no + " in StaffVacancy and StaffHeadcount tables were successfully deleted by " + Session["tkusername"].ToString() + ".");
                return Json(staff, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.Log.Info("Exception:" + ex.Message + ":::" + ex.InnerException);
                throw;
            }
        }

        public JsonResult GetStaffTypeList()
        {
            var lst = db.StaffList_Fins.Select(s => s.StaffType).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBUList()
        {
            var lst = db.StaffList_Fins.Select(s => s.BU_Fin).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFunctionList()
        {
            var lst = db.StaffList_Fins.Select(s => s.Func_Fin).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobList()
        {
            var lst = db.StaffList_Fins.Select(s => s.Job_Fin).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAXCodeList()
        {
            var lst = db.StaffList_Fins.Select(s => s.AXcode).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEntityList()
        {
            var lst = db.StaffList_Fins.Select(s => s.Entity_Fin).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDivisionList()
        {
            var lst = db.StaffList_Fins.Select(s => s.Division_Fin).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTitleList()
        {
            var lst = db.StaffLists.Select(s => s.Title).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDept_DescList()
        {
            var lst = db.StaffLists.Select(s => s.Dept_Desc).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDivList()
        {
            var lst = db.StaffLists.Select(s => s.Division).Distinct().OrderBy(s => s).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public void getMonthsAndYear()
        {
            string currentFiscalYear = helper.getCurrentFiscalYear();
            dateFY = DateTime.Parse("01-" + ConfigurationManager.AppSettings["FirstMonthOfFY"] + "-" + currentFiscalYear.Substring(0, 4));

            firstMonth = dateFY.Value.ToString("MMM");
            secondMonth = dateFY.Value.AddMonths(1).ToString("MMM");
            thirdMonth = dateFY.Value.AddMonths(2).ToString("MMM");
            fourthMonth = dateFY.Value.AddMonths(3).ToString("MMM");
            fifthMonth = dateFY.Value.AddMonths(4).ToString("MMM");
            sixthMonth = dateFY.Value.AddMonths(5).ToString("MMM");
            seventhMonth = dateFY.Value.AddMonths(6).ToString("MMM");
            eighthMonth = dateFY.Value.AddMonths(7).ToString("MMM");
            ninthMonth = dateFY.Value.AddMonths(8).ToString("MMM");
            tenthMonth = dateFY.Value.AddMonths(9).ToString("MMM");
            eleventhMonth = dateFY.Value.AddMonths(10).ToString("MMM");
            twelfthMonth = dateFY.Value.AddMonths(11).ToString("MMM");

            firstYear = dateFY.Value.ToString("yyyy");
            secondYear = dateFY.Value.AddMonths(1).ToString("yyyy");
            thirdYear = dateFY.Value.AddMonths(2).ToString("yyyy");
            fourthYear = dateFY.Value.AddMonths(3).ToString("yyyy");
            fifthYear = dateFY.Value.AddMonths(4).ToString("yyyy");
            sixthYear = dateFY.Value.AddMonths(5).ToString("yyyy");
            seventhYear = dateFY.Value.AddMonths(6).ToString("yyyy");
            eighthYear = dateFY.Value.AddMonths(7).ToString("yyyy");
            ninthYear = dateFY.Value.AddMonths(8).ToString("yyyy");
            tenthYear = dateFY.Value.AddMonths(9).ToString("yyyy");
            eleventhYear = dateFY.Value.AddMonths(10).ToString("yyyy");
            twelfthYear = dateFY.Value.AddMonths(11).ToString("yyyy");
        }
    }
}