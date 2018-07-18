using System.Collections.Generic;

namespace safnetDirectory.FullMvc.Models
{
    public class EmployeeList
    {
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public int TotalRecords { get; set; }
    }
}