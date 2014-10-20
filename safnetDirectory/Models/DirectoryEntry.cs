using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace safnetDirectory.Models
{
    public class DirectoryEntry
    {
        [Key]
        public int EmployeeId { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string OfficePhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string MobilePhoneNumber { get; set; }
    }


    public class DirectoryEntryContext : DbContext
    {
        public DirectoryEntryContext()
            : base("DefaultConnection")
        {
        }

        public static DirectoryEntryContext Create()
        {
            return new DirectoryEntryContext();
        }
    }
}