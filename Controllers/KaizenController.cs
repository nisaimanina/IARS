using Microsoft.AspNetCore.Mvc;
using IARS.Data;
using IARS.Models;
using IARS.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System;
using Spire.Xls;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace IARS.Controllers
{
    public class KaizenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public KaizenController(ApplicationDbContext context, AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public IActionResult Index()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return RedirectToAction("Login", "Home");

            var proposals = _context.KaizenProposals
                .Include(p => p.Employee)
                .Include(p => p.AssignedHOD)
                .Where(p => p.EmployeeID == employeeId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.HODList = _context.Employees
                .Where(e => e.Role.ToUpper() == "HOD" || e.Role.ToUpper() == "REVIEWER")
                .OrderBy(e => e.Name)
                .ToList();

            ViewBag.FAList = _context.Employees
                .Where(e => e.Role.ToUpper() == "FINALAPPROVER")
                .OrderBy(e => e.Name)
                .ToList();

            return View(proposals);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("EmployeeID") == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return RedirectToAction("Login", "Home");

            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null || proposal.EmployeeID != employeeId) return NotFound();

            if (proposal.Status != "Pending" && !proposal.Status.Contains("Rejected"))
            {
                TempData["Error"] = "Only pending or rejected proposals can be edited.";
                return RedirectToAction("Index");
            }

            return View(proposal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KaizenProposal model, IFormFile? imageBefore, IFormFile? imageAfter, IFormFile? pdfAttachment)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return RedirectToAction("Login", "Home");

            var existing = await _context.KaizenProposals.FindAsync(id);
            if (existing == null || existing.EmployeeID != employeeId) return NotFound();

            if (existing.Status != "Pending" && !existing.Status.Contains("Rejected"))
            {
                TempData["Error"] = "Only pending or rejected proposals can be edited.";
                return RedirectToAction("Index");
            }

            existing.Title = model.Title;
            existing.Objective = model.Objective;
            existing.IsSafety = model.IsSafety;
            existing.IsQuality = model.IsQuality;
            existing.IsDelivery = model.IsDelivery;
            existing.IsCost = model.IsCost;
            existing.IsFiveS = model.IsFiveS;
            existing.IsDigital = model.IsDigital;
            existing.IsSatisfaction = model.IsSatisfaction;
            existing.IsSustainability = model.IsSustainability;
            existing.IsEngagement = model.IsEngagement;
            existing.IsSocialImpact = model.IsSocialImpact;

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "kaizen");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            if (imageBefore != null) existing.ImageBeforePath = await SaveFile(imageBefore, uploadsFolder);
            if (imageAfter != null) existing.ImageAfterPath = await SaveFile(imageAfter, uploadsFolder);
            if (pdfAttachment != null) existing.AttachmentPath = await SaveFile(pdfAttachment, uploadsFolder);

            var aiResult = await _aiService.GenerateProposal(
                existing.Title, existing.Objective, existing.ImageBeforePath, existing.ImageAfterPath,
                model.IsSafety, model.IsQuality, model.IsDelivery, model.IsCost, model.IsFiveS, model.IsDigital,
                model.IsSatisfaction, model.IsSustainability, model.IsEngagement, model.IsSocialImpact);
            
            existing.CurrentSituation = aiResult.situation;
            existing.ImprovementIdea = aiResult.idea;
            existing.ExpectedResults = aiResult.results;
            existing.Safety = aiResult.safety;
            existing.Quality = aiResult.quality;
            existing.Delivery = aiResult.delivery;
            existing.Cost = aiResult.cost;
            existing.FiveS = aiResult.fiveS;
            existing.Digital = aiResult.digital;
            existing.InvestmentCost = aiResult.investment;
            existing.VendorSupport = aiResult.vendor;
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Proposal updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KaizenProposal model, IFormFile? imageBefore, IFormFile? imageAfter, IFormFile? pdfAttachment)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            if (employeeId == null) return RedirectToAction("Login", "Home");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "kaizen");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            if (imageBefore != null) model.ImageBeforePath = await SaveFile(imageBefore, uploadsFolder);
            if (imageAfter != null) model.ImageAfterPath = await SaveFile(imageAfter, uploadsFolder);
            if (pdfAttachment != null) model.AttachmentPath = await SaveFile(pdfAttachment, uploadsFolder);

            var aiResult = await _aiService.GenerateProposal(
                model.Title, model.Objective, model.ImageBeforePath, model.ImageAfterPath,
                model.IsSafety, model.IsQuality, model.IsDelivery, model.IsCost, model.IsFiveS, model.IsDigital,
                model.IsSatisfaction, model.IsSustainability, model.IsEngagement, model.IsSocialImpact);

            model.EmployeeID = employeeId.Value;
            model.CurrentSituation = aiResult.situation;
            model.ImprovementIdea = aiResult.idea;
            model.ExpectedResults = aiResult.results;
            model.Safety = aiResult.safety;
            model.Quality = aiResult.quality;
            model.Delivery = aiResult.delivery;
            model.Cost = aiResult.cost;
            model.FiveS = aiResult.fiveS;
            model.Digital = aiResult.digital;
            model.InvestmentCost = aiResult.investment;
            model.VendorSupport = aiResult.vendor;
            model.CreatedAt = DateTime.Now;

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "KaizenTemplate.xlsx");
            if (System.IO.File.Exists(templatePath))
            {
                using (var workbook = new Workbook())
                {
                    workbook.LoadFromFile(templatePath);
                    var ws = workbook.Worksheets[0];
                    while (workbook.Worksheets.Count > 1) workbook.Worksheets.RemoveAt(workbook.Worksheets.Count - 1);
                    ws.PageSetup.FitToPagesWide = 1;
                    ws.PageSetup.FitToPagesTall = 1;
                    ws.PageSetup.IsFitToPage = true;

                    ws.Range["AB3"].Text = HttpContext.Session.GetString("UserName") ?? "";
                    ws.Range["AB4"].Text = employeeId.ToString();
                    ws.Range["AB5"].Text = HttpContext.Session.GetString("UserDept") ?? "";
                    ws.Range["Z7"].Text = model.CreatedAt.ToString("dd/MM/yyyy");

                    ws.Range["F11"].Text = model.Title;
                    ws.Range["F11"].Style.WrapText = true;
                    ws.Range["F14"].Text = model.Objective;
                    ws.Range["F14"].Style.WrapText = true;

                    ws.Range["E8"].Text = model.IsSafety ? "Yes" : "No";
                    ws.Range["I8"].Text = model.IsQuality ? "Yes" : "No";
                    ws.Range["M8"].Text = model.IsDelivery ? "Yes" : "No";
                    ws.Range["Q8"].Text = model.IsCost ? "Yes" : "No";
                    ws.Range["V8"].Text = model.IsFiveS ? "Yes" : "No";
                    ws.Range["Y8"].Text = model.IsDigital ? "Yes" : "No";
                    ws.Range["U67"].Text = model.VendorSupport ?? "";
                    ws.Range["U70"].Text = model.InvestmentCost ?? "";

                    ws.Range["E40"].Text = model.CurrentSituation ?? "";
                    ws.Range["E40"].Style.WrapText = true;
                    ws.Range["U40"].Text = model.ImprovementIdea ?? "";
                    ws.Range["U40"].Style.WrapText = true;
                    
                    string formattedResults = $"EXPECTED IMPACT:\n1. Safety: {model.Safety}\n2. Quality: {model.Quality}\n3. Delivery: {model.Delivery}\n4. Cost: {model.Cost}\n5. 5S: {model.FiveS}\n6. Digital: {model.Digital}";
                    ws.Range["U44"].Text = formattedResults;
                    ws.Range["U44"].Style.WrapText = true;

                    if (!string.IsNullOrEmpty(model.ImageBeforePath)) AddImageSpire(ws, model.ImageBeforePath, 17, 2);
                    if (!string.IsNullOrEmpty(model.ImageAfterPath)) AddImageSpire(ws, model.ImageAfterPath, 17, 18);

                    string resultFileName = $"Kaizen_Final_{Guid.NewGuid()}.xlsx";
                    string resultPath = Path.Combine(uploadsFolder, resultFileName);
                    workbook.SaveToFile(resultPath, ExcelVersion.Version2013);
                    model.ExcelFilePath = "/uploads/kaizen/" + resultFileName;
                }
            }

            _context.KaizenProposals.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProposalForm", new { id = model.Id });
        }

        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            string uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
            using (var fs = new FileStream(Path.Combine(folder, uniqueName), FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            return "/uploads/kaizen/" + uniqueName;
        }

        private void AddImageSpire(Worksheet ws, string relPath, int row, int col)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relPath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                var pic = ws.Pictures.Add(row, col, fullPath);
                pic.Width = 280; pic.Height = 190;
            }
        }

        public async Task<IActionResult> ProposalForm(int id)
        {
            var proposal = await _context.KaizenProposals
                .Include(p => p.Employee)
                .Include(p => p.AssignedHOD)
                .Include(p => p.FinalApproverEmployee)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (proposal == null) return NotFound();
            return View("ProposalForm", proposal);
        }

        public async Task<IActionResult> Details(int id) { return await ProposalForm(id); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            DeleteFile(wwwroot, proposal.ImageBeforePath);
            DeleteFile(wwwroot, proposal.ImageAfterPath);
            DeleteFile(wwwroot, proposal.AttachmentPath);
            DeleteFile(wwwroot, proposal.ExcelFilePath);
            _context.KaizenProposals.Remove(proposal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void DeleteFile(string root, string? relPath)
        {
            if (string.IsNullOrEmpty(relPath)) return;
            string fullPath = Path.Combine(root, relPath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }

        public async Task<IActionResult> DownloadCombined(int id)
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            using (var outDoc = new PdfDocument())
            {
                if (!string.IsNullOrEmpty(proposal.ExcelFilePath))
                {
                    var excelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", proposal.ExcelFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(excelPath))
                    {
                        using (var workbook = new Workbook())
                        {
                            workbook.LoadFromFile(excelPath);
                            while (workbook.Worksheets.Count > 1) workbook.Worksheets.RemoveAt(workbook.Worksheets.Count - 1);
                            var ws = workbook.Worksheets[0];
                            ws.PageSetup.FitToPagesWide = 1; ws.PageSetup.FitToPagesTall = 1; ws.PageSetup.IsFitToPage = true;
                            workbook.ConverterSetting.SheetFitToPage = true;
                            using (var ms = new MemoryStream())
                            {
                                workbook.SaveToStream(ms, FileFormat.PDF); ms.Position = 0;
                                using (var excelPdf = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
                                {
                                    if (excelPdf.Pages.Count > 0) outDoc.AddPage(excelPdf.Pages[0]);
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(proposal.AttachmentPath))
                {
                    var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", proposal.AttachmentPath.TrimStart('/'));
                    if (System.IO.File.Exists(pdfPath))
                    {
                        using (var stream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
                        {
                            using (var attachmentPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
                            {
                                foreach (PdfPage page in attachmentPdf.Pages) outDoc.AddPage(page);
                            }
                        }
                    }
                }
                var finalStream = new MemoryStream();
                outDoc.Save(finalStream);
                return File(finalStream.ToArray(), "application/pdf", $"Kaizen_Combined_{id}.pdf");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitProposal(int id, int hodId, int faId)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeID");
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null || proposal.EmployeeID != employeeId) return Forbid();
            proposal.AssignedHODID = hodId;
            proposal.FinalApproverEmployeeID = faId;
            proposal.Status = "UnderReview";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewApprove(int id, string? remarks)
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            proposal.Status = "HODApproved";
            proposal.HODRemarks = remarks;
            proposal.HODReviewedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewReject(int id, string? remarks)
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            proposal.Status = "HODRejected";
            proposal.HODRemarks = remarks;
            proposal.HODReviewedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalApprove(int id, string? remarks)
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            proposal.Status = "FinalApproved";
            proposal.FinalApprovedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(int id)
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            proposal.Status = "Released";
            proposal.ReleasedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("AllProposals", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWinner(int id, string source = "AllProposals")
        {
            var proposal = await _context.KaizenProposals.FindAsync(id);
            if (proposal == null) return NotFound();
            proposal.IsWinner = !proposal.IsWinner;
            await _context.SaveChangesAsync();
            if (source == "Winners") return RedirectToAction("Winners", "Home");
            return RedirectToAction("AllProposals", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAllHistory()
        {
            var role = HttpContext.Session.GetString("UserRole")?.ToUpper();
            if (role != "ADMIN" && role != "KAIZENCOMMITTEE") return Forbid();
            _context.KaizenProposals.RemoveRange(_context.KaizenProposals);
            _context.History.RemoveRange(_context.History);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Evaluate(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "KaizenCommittee" && role != "Admin") return Unauthorized();
            var proposal = await _context.KaizenProposals
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (proposal == null) return NotFound();
            return View(proposal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluate(int id, KaizenProposal model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            if (role != "KaizenCommittee" && role != "Admin") return Unauthorized();
            var existing = await _context.KaizenProposals.FindAsync(id);
            if (existing == null) return NotFound();
            existing.IsCommitteeKaizen = model.IsCommitteeKaizen;
            existing.ScoreIdea = model.ScoreIdea;
            existing.ScoreEffort = model.ScoreEffort;
            existing.ScoreApplication = model.ScoreApplication;
            existing.ScoreImprovement = model.ScoreImprovement;
            existing.ScoreSafety = model.ScoreSafety;
            existing.ScoreTangible = model.ScoreTangible;
            existing.TotalCommitteeScore = (existing.ScoreIdea ?? 0) + (existing.ScoreEffort ?? 0) + (existing.ScoreApplication ?? 0) + (existing.ScoreImprovement ?? 0) + (existing.ScoreSafety ?? 0) + (existing.ScoreTangible ?? 0);
            if (existing.TotalCommitteeScore >= 81) existing.CommitteeRank = "A";
            else if (existing.TotalCommitteeScore >= 51) existing.CommitteeRank = "B";
            else if (existing.TotalCommitteeScore >= 31) existing.CommitteeRank = "C";
            else existing.CommitteeRank = "D";
            if (existing.ReceivedDate == null) { existing.ReceivedDate = DateTime.Now; existing.ReceivedBy = userName; }
            existing.CheckedDate = DateTime.Now; existing.CheckedBy = userName;
            await _context.SaveChangesAsync();
            _context.History.Add(new History { EmployeeID = HttpContext.Session.GetInt32("EmployeeID") ?? 0, Action = "Evaluate", Description = $"Committee evaluated proposal #{id}. Rank: {existing.CommitteeRank}", Timestamp = DateTime.Now });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Proposal evaluated successfully!";
            return RedirectToAction("AllProposals", "Home");
        }
    }
}