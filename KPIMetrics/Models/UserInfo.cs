using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPIMetrics.Models
{
    public class UserInfo
    {
        // {"Initial":"LLMN","FullName":"Love Nuqui","Surname":"Nuqui","GivenName":"Lovenisa Manabat","Title":"IT Development Supervisor","Email":"llmn@wallem.com","Company":"Wallem Innovative Solutions Phils. Inc","Department":"Corporate/ IT"}
        public int Id { get; set; }
        public string Initial { get; set; }
        public string FullName { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string BU { get; set; }
        public int BUId { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public IEnumerable<SelectListItem> BUList { get; set; }
        public IEnumerable<SelectListItem> LocationList { get; set; }
    }
}