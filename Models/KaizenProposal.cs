using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IARS.Models
{
    public class KaizenProposal
    {
        [Key]
        public int Id { get; set; }

        public string? FormNo { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee? Employee { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Objective { get; set; } = string.Empty;

        public string? CurrentSituation { get; set; }
        public string? ImprovementIdea { get; set; }
        public string? ExpectedResults { get; set; }

        // Impact Selection (Checkboxes)
        public bool IsSafety { get; set; }
        public bool IsQuality { get; set; }
        public bool IsDelivery { get; set; }
        public bool IsCost { get; set; }
        public bool IsFiveS { get; set; }
        public bool IsDigital { get; set; }
        public bool IsSatisfaction { get; set; }
        public bool IsSustainability { get; set; }
        public bool IsEngagement { get; set; }
        public bool IsSocialImpact { get; set; }

        // Committee Evaluation Fields
        public bool? IsCommitteeKaizen { get; set; }
        public int? ScoreIdea { get; set; }
        public int? ScoreEffort { get; set; }
        public int? ScoreApplication { get; set; }
        public int? ScoreImprovement { get; set; }
        public int? ScoreSafety { get; set; }
        public int? ScoreTangible { get; set; }
        public int? TotalCommitteeScore { get; set; }
        public string? CommitteeRank { get; set; }
        
        public string? ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? CheckedBy { get; set; }
        public DateTime? CheckedDate { get; set; }

        // 5S Evaluation (AI Generated Details)
        public string? Safety { get; set; }
        public string? Quality { get; set; }
        public string? Delivery { get; set; }
        public string? Cost { get; set; }
        public string? FiveS { get; set; }
        public string? Digital { get; set; }
        public string? InvestmentCost { get; set; }
        public string? VendorSupport { get; set; }

        public string? ImageBeforePath { get; set; }
        public string? ImageAfterPath { get; set; }
        public string? AttachmentPath { get; set; }
        public string? TemplatePath { get; set; }
        public string? ExcelFilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ══════════════════════════════════════════
        //  REVIEW & APPROVAL FLOW
        // ══════════════════════════════════════════

        /// <summary>
        /// Pending → UnderReview → HODApproved / HODRejected
        /// → FinalApproved / FinalRejected → Released / Winner
        /// </summary>
        public string Status { get; set; } = "Pending";

        // Level 1 — HOD Review
        public int? AssignedHODID { get; set; }

        [ForeignKey("AssignedHODID")]
        public virtual Employee? AssignedHOD { get; set; }

        public string? HODRemarks { get; set; }
        public DateTime? HODReviewedAt { get; set; }

        // Level 2 — Final Approval
        public int? FinalApproverEmployeeID { get; set; }

        [ForeignKey("FinalApproverEmployeeID")]
        public virtual Employee? FinalApproverEmployee { get; set; }

        public string? FinalRemarks { get; set; }
        public DateTime? FinalApprovedAt { get; set; }

        // Kaizen Community
        public bool IsWinner { get; set; } = false;
        public DateTime? ReleasedAt { get; set; }
    }
}