using Microsoft.AspNetCore.Mvc;
using IARS.Data;
using IARS.Models;
using IARS.Services;
using ClosedXML.Excel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;

namespace IARS.Controllers
{
    public class ProposalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;
        private readonly IWebHostEnvironment _env;

        public ProposalController(ApplicationDbContext context, AIService aiService, IWebHostEnvironment env)
        {
            _context = context;
            _aiService = aiService;
            _env = env;
        }

        public IActionResult Dashboard()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Index", "Home");

            var employee = _context.Employees.FirstOrDefault(e => e.Email == userEmail);
            if (employee == null) return NotFound("Employee not found.");

            var proposals = _context.KaizenProposals
                .Where(p => p.EmployeeID == employee.EmployeeID)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(proposals);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(string title, string objective)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Index", "Home");

            var employee = _context.Employees.FirstOrDefault(e => e.Email == userEmail);
            if (employee == null) return NotFound("Employee not found.");

            // 1. Panggil AI untuk jana kandungan berdasarkan Title & Objective
            var aiResult = await _aiService.GenerateProposal(title, objective);

            // 2. Simpan rekod ke dalam Database
            var proposal = new KaizenProposal
            {
                EmployeeID = employee.EmployeeID,
                CurrentSituation = aiResult.situation,
                ImprovementIdea = aiResult.idea,
                CreatedAt = DateTime.Now
            };

            _context.KaizenProposals.Add(proposal);
            _context.History.Add(new History
            {
                EmployeeID = employee.EmployeeID,
                Action = $"Generated Kaizen: {title}",
                Timestamp = DateTime.Now
            });
            _context.SaveChanges();

            // 3. Proses pengisian Template Excel
            string templatePath = Path.Combine(_env.ContentRootPath, "Templates", "KaizenTemplate.xlsx");

            if (!System.IO.File.Exists(templatePath))
            {
                TempData["Error"] = "Template file not found.";
                return RedirectToAction("Dashboard");
            }

            using (var workbook = new XLWorkbook(templatePath))
            {
                var worksheet = workbook.Worksheet(1);

                // --- MAKLUMAT STAF ---
                worksheet.Cell("AB3").Value = employee.Name;
                worksheet.Cell("AB4").Value = employee.EmployeeID;
                worksheet.Cell("AB5").Value = employee.Department;
                worksheet.Cell("Z7").Value = DateTime.Now.ToString("dd/MM/yyyy");

                // --- TAJUK & OBJEKTIF ---
                worksheet.Cell("F11").Value = title.ToUpper();
                worksheet.Cell("A13").Value = objective; // Kotak Theme/Objective

                // --- SEBELUM (BEFORE / CURRENT SITUATION) ---
                // Kita masukkan isi sahaja supaya tak bertindih dengan label dalam template
                worksheet.Cell("E40").Value = aiResult.situation; 
                worksheet.Cell("E44").Value = ""; // Dikosongkan (Clean look)
                worksheet.Cell("E48").Value = "";
                worksheet.Cell("E52").Value = "";
                worksheet.Cell("E57").Value = "";
                
                // --- SELEPAS (AFTER / IMPROVEMENT IDEA & RESULTS) ---
                worksheet.Cell("U40").Value = aiResult.idea;            
                worksheet.Cell("U44").Value = aiResult.results;         
                worksheet.Cell("U48").Value = aiResult.safety;    
                worksheet.Cell("U52").Value = aiResult.quality;         
                worksheet.Cell("U57").Value = aiResult.delivery;  
                worksheet.Cell("U60").Value = aiResult.cost; 
                worksheet.Cell("U63").Value = aiResult.fiveS;          
                worksheet.Cell("U67").Value = aiResult.vendor; 
                worksheet.Cell("U70").Value = aiResult.investment; 
                // Removed redundant U67 set to empty

                // --- LOGIK CHECKBOX (Pangkah '/' jika rating tinggi) ---
                // --- LOGIK CHECKBOX (Pangkah '/' jika ada impak) ---
                if (!string.IsNullOrEmpty(aiResult.safety) && !aiResult.safety.Contains("No impact")) worksheet.Cell("E8").Value = "/";
                if (!string.IsNullOrEmpty(aiResult.quality) && !aiResult.quality.Contains("No impact")) worksheet.Cell("I8").Value = "/";
                if (!string.IsNullOrEmpty(aiResult.delivery) && !aiResult.delivery.Contains("No impact")) worksheet.Cell("M8").Value = "/";
                if (!string.IsNullOrEmpty(aiResult.cost) && !aiResult.cost.Contains("No impact")) worksheet.Cell("Q8").Value = "/";
                if (!string.IsNullOrEmpty(aiResult.fiveS) && !aiResult.fiveS.Contains("No impact")) worksheet.Cell("V8").Value = "/";
                if (!string.IsNullOrEmpty(aiResult.digital) && !aiResult.digital.Contains("No impact")) worksheet.Cell("Y8").Value = "/";

                // --- KEMASKINI FORMATTING (Wrap Text) ---
                // Supaya ayat AI yang panjang tidak melimpah keluar dari kotak
                var contentRange = worksheet.Range("A11:AC68");
                contentRange.Style.Alignment.WrapText = true;
                contentRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                // 4. Penukaran Excel ke PDF
                using (var excelStream = new MemoryStream())
                {
                    workbook.SaveAs(excelStream);
                    excelStream.Position = 0;
                    
                    // Gunakan Spire.XLS untuk conversion
                    Spire.Xls.Workbook spireWorkbook = new Spire.Xls.Workbook();
                    spireWorkbook.LoadFromStream(excelStream);
                    
                    // Pastikan output PDF muat satu muka surat A4
                    spireWorkbook.ConverterSetting.SheetFitToPage = true;
                    var sheet = spireWorkbook.Worksheets[0];
                    sheet.PageSetup.PaperSize = Spire.Xls.PaperSizeType.PaperA4;
                    
                    // Set Landscape jika template Isuzu anda melintang
                    sheet.PageSetup.Orientation = Spire.Xls.PageOrientationType.Landscape;

                    using (var pdfStream = new MemoryStream())
                    {
                        spireWorkbook.SaveToStream(pdfStream, Spire.Xls.FileFormat.PDF);
                        var fileBytes = pdfStream.ToArray();
                        string fileName = $"Kaizen_{title.Replace(" ", "_")}.pdf";

                        return File(fileBytes, "application/pdf", fileName);
                    }
                }
            }
        }
    }
}