using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IARS.Models
{
    [Table("MasterTasks")] // Replace with your actual table name in SQL Server
    public class MasterTask
    {
        [Key]
        public int Id { get; set; }

        public int No { get; set; }

        [StringLength(50)]
        public string EmployeeNo { get; set; } = string.Empty;

        [Column("Project")]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        public DateTime PlanStart { get; set; }
        public DateTime PlanEnd { get; set; }

        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }

        [StringLength(50)]
        public string ReferenceNo { get; set; } = string.Empty;

        [StringLength(1000)]
        public string ProjectObjective { get; set; } = string.Empty;

        [StringLength(500)]
        public string CurrentStatus { get; set; } = string.Empty;

        [StringLength(1000)]
        public string ExpectedOutcome { get; set; } = string.Empty;
    }
}