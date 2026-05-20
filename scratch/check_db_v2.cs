
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IARS.Data;
using IARS.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IARS.Scratch
{
    public class CheckDb
    {
        public static void Run(ApplicationDbContext context)
        {
            var proposals = context.KaizenProposals.Include(p => p.Employee).ToList();
            Console.WriteLine($"Total Proposals: {proposals.Count}");
            foreach (var p in proposals)
            {
                Console.WriteLine($"ID: {p.Id}, Title: {p.Title}, EmployeeID: {p.EmployeeID}, EmployeeName: {p.Employee?.Name ?? "NULL"}");
            }

            var employees = context.Employees.ToList();
            Console.WriteLine($"\nTotal Employees: {employees.Count}");
            foreach (var e in employees)
            {
                Console.WriteLine($"ID: {e.EmployeeID}, Name: {e.Name}, Dept: {e.Department}");
            }
        }
    }
}
