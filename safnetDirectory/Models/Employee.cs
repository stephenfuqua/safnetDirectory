using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace safnetDirectory.FullMvc.Models
{
    public class Employee
    {
        [Key]
        public string Id { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile number")]
        public string MobilePhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Office number")]
        public string OfficePhoneNumber { get; set; }

        [Display(Name = "HR User")]
        public bool IsHrUser { get; set; }
    }
}