using IARS.Data;
using IARS.Models;

namespace IARS.Services
{
    /// <summary>
    /// Centralised service for creating notifications.
    /// Call SendAsync() from any controller action that should trigger a notification.
    /// </summary>
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates and persists a single notification for one recipient.
        /// </summary>
        public async Task SendAsync(
            int recipientEmployeeId,
            string title,
            string message,
            string icon = "bi-bell-fill",
            string iconBg = "#eff6ff",
            string iconColor = "#2563eb",
            string? linkUrl = null)
        {
            if (recipientEmployeeId <= 0) return;

            var notif = new Notification
            {
                RecipientEmployeeID = recipientEmployeeId,
                Title   = title,
                Message = message,
                Icon    = icon,
                IconBg  = iconBg,
                IconColor = iconColor,
                LinkUrl = linkUrl,
                IsRead  = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sends the same notification to multiple recipients at once.
        /// </summary>
        public async Task SendToManyAsync(
            IEnumerable<int> recipientIds,
            string title,
            string message,
            string icon = "bi-bell-fill",
            string iconBg = "#eff6ff",
            string iconColor = "#2563eb",
            string? linkUrl = null)
        {
            foreach (var id in recipientIds.Where(i => i > 0).Distinct())
            {
                _context.Notifications.Add(new Notification
                {
                    RecipientEmployeeID = id,
                    Title   = title,
                    Message = message,
                    Icon    = icon,
                    IconBg  = iconBg,
                    IconColor = iconColor,
                    LinkUrl = linkUrl,
                    IsRead  = false,
                    CreatedAt = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
