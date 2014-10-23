using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace safnetDirectory.FullMvc.Models
{

    public class EmployeeViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string email { get; set; }
        public string office { get; set; }
        public string mobile { get; set; }
    }
}