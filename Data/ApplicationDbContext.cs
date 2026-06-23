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
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary FK — Employee who submitted (Cascade OK)
            modelBuilder.Entity<KaizenProposal>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Level 1 — Reviewer (NoAction to avoid multiple cascade paths)
            modelBuilder.Entity<KaizenProposal>()
                .HasOne(p => p.Reviewer)
                .WithMany()
                .HasForeignKey(p => p.ReviewerID)
                .OnDelete(DeleteBehavior.NoAction);

            // Level 2 — Final Approver (NoAction to avoid multiple cascade paths)
            modelBuilder.Entity<KaizenProposal>()
                .HasOne(p => p.FinalApprover)
                .WithMany()
                .HasForeignKey(p => p.FinalApproverID)
                .OnDelete(DeleteBehavior.NoAction);

            // History — Employee (Cascade)
            modelBuilder.Entity<History>()
                .HasOne(h => h.Employee)
                .WithMany()
                .HasForeignKey(h => h.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification — Recipient Employee (NoAction to avoid cascade conflicts)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Recipient)
                .WithMany()
                .HasForeignKey(n => n.RecipientEmployeeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}