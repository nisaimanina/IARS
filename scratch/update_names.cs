using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IARS.Data;
using IARS.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IARS.Scratch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                Console.WriteLine("Updating employee names...");

                var admin = context.Employees.FirstOrDefault(e => e.Email == "admin@iars.com");
                if (admin != null) admin.Name = "Admin Manager";

                var ali = context.Employees.FirstOrDefault(e => e.Email == "ali@iars.com");
                if (ali != null) ali.Name = "Ali Muhammad Amran";

                var ahmad = context.Employees.FirstOrDefault(e => e.Email == "ahmad@iars.com");
                if (ahmad != null) ahmad.Name = "Ahmad Yusri Mahmoud";

                var john = context.Employees.FirstOrDefault(e => e.Email == "john@iars.com");
                if (john != null) john.Name = "John Sullivan";

                int count = context.SaveChanges();
                Console.WriteLine($"Successfully updated {count} records.");
            }
        }
    }
}
