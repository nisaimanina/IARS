using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IARS.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Employee yang menerima notifikasi ini.</summary>
        [Required]
        public int RecipientEmployeeID { get; set; }

        [ForeignKey("RecipientEmployeeID")]
        public virtual Employee? Recipient { get; set; }

        /// <summary>Tajuk ringkas notifikasi.</summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Badan mesej notifikasi.</summary>
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        /// <summary>Bootstrap icon class, e.g. "bi-check-circle-fill".</summary>
        [MaxLength(80)]
        public string Icon { get; set; } = "bi-bell-fill";

        /// <summary>CSS background colour for icon box, e.g. "#ecfdf5".</summary>
        [MaxLength(20)]
        public string IconBg { get; set; } = "#eff6ff";

        /// <summary>CSS text colour for icon, e.g. "#059669".</summary>
        [MaxLength(20)]
        public string IconColor { get; set; } = "#2563eb";

        /// <summary>Optional URL to navigate to when notification is clicked.</summary>
        [MaxLength(300)]
        public string? LinkUrl { get; set; }

        /// <summary>False = belum dibaca (red badge), True = sudah dibaca.</summary>
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
