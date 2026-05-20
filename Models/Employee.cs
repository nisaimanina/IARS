using System;
using System.ComponentModel.DataAnnotations;

namespace IARS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Role { get; set; } = "Employee";

        public DateTime FirstLoginDate { get; set; } = DateTime.Now;

        public DateTime LastLoginDate { get; set; } = DateTime.Now;
    }
}