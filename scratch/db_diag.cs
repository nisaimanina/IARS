using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IARS.Data;
using IARS.Models;
using Microsoft.Extensions.DependencyInjection;
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
                try 
                {
                    var proposals = context.KaizenProposals
                        .Include(p => p.Employee)
                        .ToList();
                    Console.WriteLine($"Found {proposals.Count} proposals.");
                    foreach (var p in proposals)
                    {
                        Console.WriteLine($"P{p.Id}: EmployeeID={p.EmployeeID}, EmployeeName={p.Employee?.Name ?? "NULL"}");
                    }
                    Console.WriteLine("DB Check Successful.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DB Error: {ex.Message}");
                    if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
                }
            }
        }
    }
}
