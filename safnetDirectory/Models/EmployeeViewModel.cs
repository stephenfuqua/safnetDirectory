using System.ComponentModel.DataAnnotations;

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

        public string edit
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    // TODO: This is terrible. used by Razor
                    return "<div onclick=\"" + id + "class=\"ui-icon ui-icon-wrench\"></div>";
                }

                return "";
            }
        }
    }
}