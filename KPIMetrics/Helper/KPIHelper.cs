using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KPIMetrics.Sso;
using KPIMetrics.Models;
using System.Globalization;
using System.Configuration;

namespace KPIMetrics.Models
{
    public class KPIHelper
    {
        Models.KPIMetrics db = new Models.KPIMetrics();

        public List<string> checkEditAll()
        {
            List<string> rfList = new List<string>();
            var systemDate = "";

            if (ConfigurationManager.AppSettings["DebugSystemDate"] != "")
            {
                systemDate = ConfigurationManager.AppSettings["DebugSystemDate"];
            }
            else
            {
                systemDate = DateTime.Now.ToString("dd-MMM-yyyy");
            }

            string currentFinYear = getCurrentFiscalYear();
            DateTime sysDate = DateTime.Parse(systemDate);

            rfList = (from r in db.KPI_ControlPeriods
                      where r.FinYear == currentFinYear && r.StartDate <= sysDate && r.EndDate >= sysDate && (r.Model == "RF1" || r.Model == "RF2" || r.Model == "RF3")
                      select r.Model).ToList();

            return rfList;
        }


        public string getCurrentFiscalYear()
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

            var monthToday = getMonthNum(systemMonth);
            var currentFinYear = monthToday <= 3 ? systemDate.Substring(7, 4) + "-" + (int.Parse(systemDate.Substring(7, 4)) + 1).ToString().Substring(2, 2) : (int.Parse(systemDate.Substring(7, 4)) - 1).ToString() + "-" + systemDate.Substring(9, 2);

            return currentFinYear;
        }

        private int getMonthNum(string month)
        {
            int monthNo = 0;
            month = month.ToUpper();
            DateTime dt = DateTime.Parse("2022-" + ConfigurationManager.AppSettings["FirstMonthOfFY"].ToUpper() + "-01");

            for (int i = 1; i <= 12; i++)
            {
                if (dt.ToString("MMM").ToUpper() == month)
                {
                    monthNo = i;
                    break;
                }

                dt = dt.AddMonths(1);
            }

            return monthNo;
        }

        public bool isEditable(string model, string finYear, string calMonth, string calYear)
        {
            bool isEditable = false;
            var systemDate = "";
            var recDate = DateTime.Parse(calYear + "-" + calMonth + "-1");

            if (ConfigurationManager.AppSettings["DebugSystemDate"] != "")
            {
                systemDate = ConfigurationManager.AppSettings["DebugSystemDate"];
            }
            else
            {
                systemDate = DateTime.Now.ToString("dd-MMM-yyyy");
            }

            string currentFinYear = getCurrentFiscalYear();
            var calendarYear = calYear;
            //int monthToday = int.Parse(GetMonthNum(systemMonth).ToString("00"));

            List<KPI_ControlPeriod> refValue = (from r in db.KPI_ControlPeriods
                                                where r.FinYear == finYear
                                                select r).ToList();


            if (model != "")
            {
                refValue = refValue.Where(r => r.Model == model).Select(r => r).ToList();
                if (refValue.Count() > 0)
                {
                    //int monthFrom = int.Parse(refValue.FirstOrDefault().StartDate.ToString("yyyy") + GetMonthNum(refValue.FirstOrDefault().StartDate.ToString("MMM")).ToString("00"));
                    //int monthTo = int.Parse(refValue.FirstOrDefault().StartDate.ToString("yyyy") + GetMonthNum(refValue.FirstOrDefault().EndDate.ToString("MMM")).ToString("00"));
                    DateTime start = refValue.FirstOrDefault().StartDate;
                    DateTime end = refValue.FirstOrDefault().EndDate;
                    DateTime recordStart = refValue.FirstOrDefault().RecordStartDate;
                    DateTime recordEnd = refValue.FirstOrDefault().RecordEndDate;

                    // if (((currentFinYear == financialYear && GetMonthNum(calMonth) >= monthToday) || int.Parse(systemDate.Substring(7, 4)) < int.Parse(calYear) || int.Parse(systemDate.Substring(7, 4)) == int.Parse(calYear)) && (monthFrom <= monthToday && monthTo >= monthToday) && (GetMonthNum(calMonth) >= monthToday || (int.Parse(systemDate.Substring(7, 4)) <= int.Parse(calYear) && GetMonthNum(calMonth) <= 3)))
                    if (start <= DateTime.Parse(systemDate) && end >= DateTime.Parse(systemDate) && (recordStart <= recDate && recordEnd >= recDate))
                    {
                        isEditable = true;
                    }
                }
                else
                {
                    isEditable = false;
                }
            }
            else //for the edit button
            {
                //if (((currentFinYear == financialYear && GetMonthNum(calMonth) >= monthToday) || int.Parse(systemDate.Substring(7, 4)) < int.Parse(calYear) || int.Parse(systemDate.Substring(7, 4)) == int.Parse(calYear)) && (GetMonthNum(calMonth) >= monthToday || (int.Parse(systemDate.Substring(7, 4)) <= int.Parse(calYear) && GetMonthNum(calMonth) <= 3)))
                foreach (var item in refValue)
                {
                    DateTime start = item.StartDate;
                    DateTime end = item.EndDate;
                    DateTime recordStart = item.RecordStartDate;
                    DateTime recordEnd = item.RecordEndDate;

                    // if (int.Parse(currentFinYear.Substring(0,4)) < int.Parse(financialYear.Substring(0,4)) || (int.Parse(currentFinYear.Substring(0, 4)) == int.Parse(financialYear.Substring(0, 4)) && start <= DateTime.Parse(systemDate) && end >= DateTime.Parse(systemDate)) && (recordStart <= recDate && recordEnd >= recDate))
                    if (start <= DateTime.Parse(systemDate) && end >= DateTime.Parse(systemDate) && (recordStart <= recDate && recordEnd >= recDate))
                    {
                        isEditable = true;
                        break;
                    }
                }
            }


            return isEditable;
        }

        public string getMonthAbbr(int month)
        {
            string monthAbbr = "";
            DateTime dt = DateTime.Parse("2022-" + ConfigurationManager.AppSettings["FirstMonthOfFY"].ToUpper() + "-01");

            for (int i = 1; i <= 12; i++)
            {
                if (i == month)
                {
                    monthAbbr = dt.ToString("MMM");
                    break;
                }

                dt = dt.AddMonths(1);
            }

            return monthAbbr;
        }

        public List<SelectListItem> getMonth(string SelectedValue)
        {
            List<SelectListItem> monthList = new List<SelectListItem>();
            DateTime dt = DateTime.Parse("2022-" + ConfigurationManager.AppSettings["FirstMonthOfFY"].ToUpper() + "-01");

            for (int i = 1; i <= 12; i++)
            {
                var data = new SelectListItem { Value = i.ToString(), Text = dt.ToString("MMMM") };
                monthList.Add(data);
                dt = dt.AddMonths(1);
            }

            //monthList = data.ToList();

            return monthList.ToList();
        }

        public List<SelectListItem> getLineFunction(string BU, string metricName)
        {
            int buId = (from b in db.KPI_BUs
                        where b.Name == BU
                        select b.ID).FirstOrDefault();

            int metricId = (from m in db.KPI_Metrics
                            where m.MetricName == metricName
                            select m.ID).FirstOrDefault();

            var lfList = (from l in db.KPI_Line_Functions
                          where l.BUId == buId && l.MetricId == metricId
                          select new SelectListItem { Value = l.ID.ToString(), Text = l.Name }).ToList<SelectListItem>();


            //if (lfList.Count > 1)
            //{
            //    lfList.Insert(0, new SelectListItem() { Value = "All", Text = "All" });

            //}

            return lfList.ToList();

        }


        public string[] getLineFunctionNames(int BU, string metricName)
        {
            string[] LOB = null;            
            int metricId = db.KPI_Metrics.Where(m => m.MetricName == metricName).Select(m => m.ID).FirstOrDefault();

            LOB = (from l in db.KPI_Line_Functions
                   where l.BUId == BU && l.MetricId == metricId
                   select l.Name).ToArray();

            return LOB;

        }
    }
}