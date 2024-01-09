using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPIMetrics.Models
{
    public class KPIClass
    {
        public IEnumerable<SelectListItem> KPI1List { get; set; }
        public IEnumerable<SelectListItem> KPI2List { get; set; }
        public IEnumerable<SelectListItem> KPI3List { get; set; }
        public IEnumerable<SelectListItem> KPI4List { get; set; }       
        public IEnumerable<SelectListItem> ModelList { get; set; }          
        public IEnumerable<SelectListItem> MonthList { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> FlightCallTypeList { get; set; }
        public IEnumerable<SelectListItem> PerformingAgentList { get; set; }
        public IEnumerable<SelectListItem> DryDockingList { get; set; }
        public IEnumerable<SelectListItem> LineFunctionList { get; set; }
        public int Id { get; set; }
        public int SetId { get; set; }
        public int MetricId { get; set; }
        public string BU { get; set; }
        public string LOB { get; set; }
        public string CalYear { get; set; }
        public string CalMonth { get; set; }
        public DateTime CalMonth2 { get; set; }
        public string FinYear { get; set; }
        public string Model { get; set; }
        public string Customer { get; set; }
        public string KPI1 { get; set; }
        public string KPI2 { get; set; }
        public string KPI3 { get; set; }
        public string KPI4 { get; set; }      
        public decimal? BGT { get; set; }
        public decimal? ACT { get; set; }
        public decimal? RF1 { get; set; }
        public decimal? RF2 { get; set; }
        public decimal? RF3 { get; set; }
        public decimal? Measure1 { get; set; }
        public int Measure2 { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string UpdatedBy { get; set; }
        public string Currency { get; set; }     
    }
}