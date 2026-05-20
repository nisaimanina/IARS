using Microsoft.EntityFrameworkCore;
using IARS.Models;

namespace IARS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<MasterTask> MasterTasks { get; set; }
        public DbSet<KaizenProposal> KaizenProposals { get; set; }
        public DbSet<History> History { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary FK — Employee who submitted (Cascade OK)
            modelBuilder.Entity<KaizenProposal>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Level 1 — Assigned HOD (NoAction to avoid multiple cascade paths)
            modelBuilder.Entity<KaizenProposal>()
                .HasOne(p => p.AssignedHOD)
                .WithMany()
                .HasForeignKey(p => p.AssignedHODID)
                .OnDelete(DeleteBehavior.NoAction);

            // Level 2 — Final Approver (NoAction to avoid multiple cascade paths)
            modelBuilder.Entity<KaizenProposal>()
                .HasOne(p => p.FinalApproverEmployee)
                .WithMany()
                .HasForeignKey(p => p.FinalApproverEmployeeID)
                .OnDelete(DeleteBehavior.NoAction);

            // History — Employee (Cascade)
            modelBuilder.Entity<History>()
                .HasOne(h => h.Employee)
                .WithMany()
                .HasForeignKey(h => h.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}