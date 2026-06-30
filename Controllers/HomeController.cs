using IARS.Models;
using IARS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IARS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string FIXED_PASSWORD = "iars2026";

        public class DeptStat { 
            public string Dept { get; set; } = "";
            public int Count { get; set; }
            public int Approved { get; set; }
        }

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── GET /Home/Login ──────────────────────────────────────
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("EmployeeID") != null)
                return RedirectToAction("Index");
            return View();
        }

        // ── POST /Home/Login ─────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null || password != FIXED_PASSWORD)
            {
                ViewBag.Error = "Invalid email or password. Please try again.";
                return View();
            }

            HttpContext.Session.SetInt32("EmployeeID", employee.EmployeeID);
            HttpContext.Session.SetString("UserName", employee.Name);
            HttpContext.Session.SetString("UserRole", employee.Role);
            HttpContext.Session.SetString("UserDept", employee.Department);
            HttpContext.Session.SetString("UserEmail", employee.Email);

            // Log login
            _context.History.Add(new History
            {
                EmployeeID  = employee.EmployeeID,
                Action      = "Login",
                Description = $"{employee.Name} ({employee.Role}) has logged in.",
                Timestamp   = DateTime.Now
            });
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ── GET /Home/Index ── (Dashboard — role-based) ──────────
        public IActionResult Index()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return RedirectToAction("Login");

            var role = HttpContext.Session.GetString("UserRole") ?? "User";

            return role.ToUpper() switch
            {
                "HOD"            => BuildReviewerDashboard(employeeId.Value),
                "REVIEWER"       => BuildReviewerDashboard(employeeId.Value),
                "FINALAPPROVER"  => BuildApproverDashboard(employeeId.Value),
                "KAIZENCOMMITTEE"=> BuildCommitteeDashboard(),
                "ADMIN"          => BuildCommitteeDashboard(),
                _                => BuildUserDashboard(employeeId.Value)
            };
        }

        // ── User (Employee) Dashboard ────────────────────────────
        private IActionResult BuildUserDashboard(int employeeId)
        {
            var proposals = _context.KaizenProposals
                .Include(p => p.Employee)
                .Where(p => p.EmployeeID == employeeId)
                .ToList();

            ViewBag.TotalProposals    = proposals.Count;
            ViewBag.ApprovedProposals = proposals.Count(p => p.Status == "FinalApproved" || p.Status == "Released" || p.IsWinner);
            ViewBag.PendingProposals  = proposals.Count(p => p.Status == "Pending" || p.Status == "UnderReview");
            ViewBag.RejectedProposals = proposals.Count(p => p.Status == "HODRejected" || p.Status == "FinalRejected");

            ViewBag.RecentActivity = _context.History
                .Where(h => h.EmployeeID == employeeId)
                .OrderByDescending(h => h.Timestamp)
                .Take(5)
                .ToList();

            return View();  // → Views/Home/Index.cshtml
        }

        // ── POST /Home/ClearMyActivity ───────────────────────────
        // Deletes only the current user's activity history entries.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearMyActivity()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return Unauthorized();

            var rows = _context.History.Where(h => h.EmployeeID == employeeId);
            _context.History.RemoveRange(rows);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ── Reviewer Dashboard ─────────────────────────────
        private IActionResult BuildReviewerDashboard(int employeeId)
        {
            var assigned = _context.KaizenProposals
                .Include(p => p.Employee)
                .Include(p => p.FinalApprover)
                .Where(p => p.ReviewerID == employeeId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();


            ViewBag.PendingReview  = assigned.Count(p => p.Status == "UnderReview");
            ViewBag.ApprovedByMe   = assigned.Count(p => p.Status == "HODApproved" || p.Status == "FinalApproved" || p.Status == "Released");
            ViewBag.RejectedByMe   = assigned.Count(p => p.Status == "HODRejected");
            ViewBag.TotalAssigned  = assigned.Count;

            return View("ReviewerDashboard", assigned);
        }

        // ── Final Approver Dashboard ─────────────────────────────
        private IActionResult BuildApproverDashboard(int employeeId)
        {
            // ALL proposals assigned to this approver (same pattern as Reviewer)
            var assigned = _context.KaizenProposals
                .Include(p => p.Employee)
                .Include(p => p.Reviewer)
                .Where(p => p.FinalApproverID == employeeId)
                .OrderByDescending(p => p.ReviewedAt)
                .ToList();

            // Pending approval (HODApproved status waiting for final approval)
            var pendingApproval = assigned.Where(p => p.Status == "HODApproved").ToList();

            // History of decisions made
            var myHistory = assigned
                .Where(p => p.Status == "FinalApproved" || p.Status == "FinalRejected" || p.Status == "Released")
                .OrderByDescending(p => p.ApprovedAt)
                .Take(10)
                .ToList();

            ViewBag.PendingApproval = assigned.Count(p => p.Status == "HODApproved");
            ViewBag.TotalAssigned   = assigned.Count;
            ViewBag.ApprovedByMe    = assigned.Count(p => p.Status == "FinalApproved" || p.Status == "Released");
            ViewBag.RejectedByMe    = assigned.Count(p => p.Status == "FinalRejected");
            ViewBag.MyHistory       = myHistory;

            return View("ApproverDashboard", pendingApproval);
        }

        // ── Kaizen Committee Dashboard (Analytics & Ranking) ─────────
        public IActionResult BuildCommitteeDashboard()
        {
            var all = _context.KaizenProposals
                .Include(p => p.Employee)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.TotalAll      = all.Count;
            ViewBag.FinalApproved = all.Count(p => p.Status == "FinalApproved" || p.Status == "Released");
            ViewBag.Released      = all.Count(p => p.Status == "Released");
            ViewBag.Winners       = all.Count(p => p.IsWinner);
            ViewBag.Pending       = all.Count(p => p.Status == "Pending" || p.Status == "UnderReview" || p.Status == "HODApproved");

            // Statistics by Department (for Analytics)
            ViewBag.DeptStats = all
                .Where(p => p.Employee != null)
                .GroupBy(p => p.Employee!.Department)
                .Select(g => new DeptStat { 
                    Dept = g.Key, 
                    Count = g.Count(), 
                    Approved = g.Count(p => p.Status == "FinalApproved" || p.Status == "Released") 
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Employee Ranking (Top Contributors)
            ViewBag.EmployeeRanking = all
                .Where(p => p.Employee != null)
                .GroupBy(p => p.Employee!)
                .Select(g => new {
                    Employee = g.Key,
                    Count = g.Count(),
                    Approved = g.Count(p => p.Status == "FinalApproved" || p.Status == "Released")
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            return View("CommitteeDashboard", all);
        }

        // ── All Proposals (For Committee) ───────────────────────────
        public IActionResult AllProposals()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "KaizenCommittee" && role != "Admin") return RedirectToAction("Index");

            var all = _context.KaizenProposals
                .Include(p => p.Employee)
                .Include(p => p.Reviewer)
                .Include(p => p.FinalApprover)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(all);
        }

        // ── Best of Month (Winners) ──────────────────────────────────
        public IActionResult Winners()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "KaizenCommittee" && role != "Admin") return RedirectToAction("Index");

            var all = _context.KaizenProposals
                .Include(p => p.Employee)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(all);
        }

        // ── Proposal History (Tracking Steps) ───────────────────────
        public IActionResult ProposalHistory()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "KaizenCommittee" && role != "Admin") return RedirectToAction("Index");

            var all = _context.KaizenProposals
                .Include(p => p.Employee)
                .Include(p => p.Reviewer)
                .Include(p => p.FinalApprover)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(all);
        }

        // ── Kaizen Management (For Actions: Release, Best of Month) ──
        public IActionResult Management()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "KaizenCommittee") return RedirectToAction("Index");

            var all = _context.KaizenProposals
                .Include(p => p.Employee)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(all);
        }

        // ── GET /Home/Logout ─────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ── GET /Home/Profile ────────────────────────────────────
        public IActionResult Profile()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return RedirectToAction("Login");

            var employee = _context.Employees.Find(employeeId);
            return View(employee);
        }
    }
}