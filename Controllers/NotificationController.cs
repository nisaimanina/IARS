using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IARS.Data;
using IARS.Models;

namespace IARS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── GET /Notification/GetUnread ──────────────────────────────────
        // Returns JSON list of unread notifications for the currently logged-in user.
        [HttpGet]
        public async Task<IActionResult> GetUnread()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return Unauthorized();

            var notifs = await _context.Notifications
                .Where(n => n.RecipientEmployeeID == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(30)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.Icon,
                    n.IconBg,
                    n.IconColor,
                    n.LinkUrl,
                    n.IsRead,
                    Time = FormatRelativeTime(n.CreatedAt)
                })
                .ToListAsync();

            var unreadCount = notifs.Count(n => !n.IsRead);

            return Json(new { items = notifs, unreadCount });
        }

        // ── POST /Notification/MarkRead/{id} ─────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return Unauthorized();

            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null || notif.RecipientEmployeeID != employeeId)
                return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ── POST /Notification/MarkAllRead ───────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return Unauthorized();

            var notifs = await _context.Notifications
                .Where(n => n.RecipientEmployeeID == employeeId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifs) n.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ── POST /Notification/ClearAll ──────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAll()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return Unauthorized();

            var notifs = await _context.Notifications
                .Where(n => n.RecipientEmployeeID == employeeId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifs);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ── Helper: relative time string ─────────────────────────────────
        private static string FormatRelativeTime(DateTime dt)
        {
            var diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 1)  return "Just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours   < 24) return $"{(int)diff.TotalHours} hour{((int)diff.TotalHours == 1 ? "" : "s")} ago";
            if (diff.TotalDays    < 7)  return $"{(int)diff.TotalDays} day{((int)diff.TotalDays == 1 ? "" : "s")} ago";
            return dt.ToString("dd MMM yyyy");
        }
    }
}
