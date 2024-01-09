using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KPIMetrics.Models
{
    public class StaffClass
    {
        public int ID { get; set; }
        public string Initial { get; set; }       
        public string emp_no { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Dept_Desc { get; set; }
        public string Division { get; set; }
        public string BranchCity { get; set; }
        public string Country { get; set; }
        public string JobFamily { get; set; }
        public string Emp_Entity { get; set; }
        public string JobCode { get; set; }
        public DateTime? DateJoined { get; set; }
        public DateTime? DateExited { get; set; }
        public string PrimaryEmail { get; set; }
        public string StaffType { get; set; }
        public string Division_Fin { get; set; }
        public string BU_Fin { get; set; }
        public string Func_Fin { get; set; }
        public string Job_Fin { get; set; }
        public string AXcode { get; set; }
        public string Entity_Fin { get; set; }
        public DateTime? LastUpdate_Fin { get; set; }
        public bool? Headcount { get; set; }
        public string HeadcountRemarks { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string MonthYearHC1 { get; set; }
        public string MonthYearHC2 { get; set; }
        public string MonthYearHC3 { get; set; }
        public string MonthYearHC4 { get; set; }
        public string MonthYearHC5 { get; set; }
        public string MonthYearHC6 { get; set; }
        public string MonthYearHC7 { get; set; }
        public string MonthYearHC8 { get; set; }
        public string MonthYearHC9 { get; set; }
        public string MonthYearHC10 { get; set; }
        public string MonthYearHC11 { get; set; }
        public string MonthYearHC12 { get; set; }
        public string MYTitle1 { get; set; } 
        public string MYTitle2 { get; set; }
        public string MYTitle3 { get; set; }
        public string MYTitle4 { get; set; }
        public string MYTitle5 { get; set; }
        public string MYTitle6 { get; set; }
        public string MYTitle7 { get; set; }
        public string MYTitle8 { get; set; }
        public string MYTitle9 { get; set; }
        public string MYTitle10 { get; set; }
        public string MYTitle11 { get; set; }
        public string MYTitle12 { get; set; }
        public bool IsDefault { get; set; }
        public string HeadcountText { get; set; }
    }
}