using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System;

namespace IARS.Services
{
    public class AIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(3);
            _apiKey = configuration["Gemini:ApiKey"] ?? "";
        }

        public async Task<(string situation, string idea, string results, string safety, string quality, string delivery, string cost, string fiveS, string digital, string investment, string vendor)> GenerateProposal(
            string title, string objective, string? imageBeforePath = null, string? imageAfterPath = null,
            bool isSafety = false, bool isQuality = false, bool isDelivery = false, bool isCost = false, 
            bool isFiveS = false, bool isDigital = false, bool isSatisfaction = false, 
            bool isSustainability = false, bool isEngagement = false, bool isSocialImpact = false)
        {
            if (string.IsNullOrEmpty(_apiKey)) return ("API Key is missing", "Error", "Error", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "0", "None");

            var impacts = new List<string>();
            if (isSafety) impacts.Add("Safety");
            if (isQuality) impacts.Add("Quality");
            if (isDelivery) impacts.Add("Delivery");
            if (isCost) impacts.Add("Cost");
            if (isFiveS) impacts.Add("5S");
            if (isDigital) impacts.Add("Digital");
            if (isSatisfaction) impacts.Add("Satisfaction");
            if (isSustainability) impacts.Add("Sustainability");
            if (isEngagement) impacts.Add("Engagement");
            if (isSocialImpact) impacts.Add("Social Impact");

            string impactContext = impacts.Count > 0 
                ? $"The user has identified that this Kaizen impacts the following areas: {string.Join(", ", impacts)}. Please focus your elaboration heavily on these areas." 
                : "Identify the most likely impact areas based on the objective.";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";


            var parts = new List<object>();
            parts.Add(new { text = $@"
                Act as an Industrial Kaizen Expert.
                TITLE: {title}
                OBJECTIVE: {objective}

                TASK:
                Analyze the provided images (if any) and objective. 
                {impactContext}
                Generate a professional and extremely concise Industrial Kaizen report.
                
                IMPORTANT: Every single point in 'situation' and 'idea' MUST be very short (strictly 1 or 2 sentences max). 
                You MUST start each line with the label followed by a colon (e.g., 'Safety: [Text]', 'Quality: [Text]') so the system can format it correctly.
                The entire report must be able to fit on a single A4 page. Do not use long paragraphs.
                
                Format 'situation' (Before) and 'idea' (After) as structured lists breaking down the condition ONLY for: 
                Safety, Quality, Delivery, Cost, 5S, and Digital.
                
                Note: Do NOT elaborate on Satisfaction, Sustainability, Engagement, or Social Impact in these sections. 
                Those are selection-only impacts.

                JSON FORMAT:
                {{
                    ""situation"": ""Safety: [Before condition]\nQuality: [Before condition]\nDelivery: [Before condition]\nCost: [Before condition]\n5S: [Before condition]\nDigital: [Before condition]"",
                    ""idea"": ""Safety: [After improvement]\nQuality: [After improvement]\nDelivery: [After improvement]\nCost: [After improvement]\n5S: [After improvement]\nDigital: [After improvement]"",
                    ""results"": ""Summary of overall expected outcomes..."",
                    ""safety"": ""Impact Score (e.g. Yes/No/High/Low)"",
                    ""quality"": ""Impact Score (e.g. Yes/No/High/Low)"",
                    ""delivery"": ""Impact Score (e.g. Yes/No/High/Low)"",
                    ""cost"": ""Impact Score (e.g. Yes/No/High/Low)"",
                    ""fiveS"": ""Impact Score (e.g. Yes/No/High/Low)"",
                    ""digital"": ""Impact Score (e.g. Yes/No/High/Low)"",
                    ""investment"": ""RM XXX (Estimate cost)"",
                    ""vendor"": ""Description of vendor involvement""
                }}" });

            if (!string.IsNullOrEmpty(imageBeforePath)) AddImagePart(parts, imageBeforePath, "Before Condition");
            if (!string.IsNullOrEmpty(imageAfterPath)) AddImagePart(parts, imageAfterPath, "After Condition");

            var requestBody = new { 
                contents = new[] { new { parts = parts.ToArray() } },
                generationConfig = new { thinkingConfig = new { thinkingBudget = 0 } }
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try 
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                
                // Check for HTTP error responses
                if (!response.IsSuccessStatusCode)
                {
                    string errorMsg = $"Gemini API Error ({(int)response.StatusCode}): ";
                    try
                    {
                        using var errorDoc = JsonDocument.Parse(responseString);
                        if (errorDoc.RootElement.TryGetProperty("error", out var error))
                        {
                            if (error.TryGetProperty("message", out var message))
                            {
                                errorMsg += message.GetString() ?? "Unknown error";
                            }
                        }
                    }
                    catch { errorMsg += responseString; }
                    
                    return (errorMsg, "Error", "Error", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "0", "None");
                }
                
                using var doc = JsonDocument.Parse(responseString);
                
                if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                {
                     return ("AI failed to generate candidates.", "Pending", "Pending", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "0", "None");
                }

                var aiText = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "{}";
                string cleanJson = aiText.Replace("```json", "").Replace("```", "").Trim();
                int jsonStart = cleanJson.IndexOf("{");
                int jsonEnd = cleanJson.LastIndexOf("}");
                if (jsonStart != -1 && jsonEnd != -1) {
                    cleanJson = cleanJson.Substring(jsonStart, jsonEnd - jsonStart + 1);
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(cleanJson, options);
                
                return (
                    result?.GetValueOrDefault("situation") ?? "Analysis pending...",
                    result?.GetValueOrDefault("idea") ?? "Improvement idea pending...",
                    result?.GetValueOrDefault("results") ?? "Expected results pending...",
                    result?.GetValueOrDefault("safety") ?? "No impact identified",
                    result?.GetValueOrDefault("quality") ?? "No impact identified",
                    result?.GetValueOrDefault("delivery") ?? "No impact identified",
                    result?.GetValueOrDefault("cost") ?? "No impact identified",
                    result?.GetValueOrDefault("fiveS") ?? "No impact identified",
                    result?.GetValueOrDefault("digital") ?? "No impact identified",
                    result?.GetValueOrDefault("investment") ?? "RM 0",
                    result?.GetValueOrDefault("vendor") ?? "Internal"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIService Exception]: {ex.ToString()}");
                return ($"Generation Error: {ex.Message} -> {ex.InnerException?.Message}", "Error", "Error", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "0", "None");
            }
        }

        private void AddImagePart(List<object> parts, string relPath, string description)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relPath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                byte[] imageBytes = File.ReadAllBytes(fullPath);
                string base64 = Convert.ToBase64String(imageBytes);
                parts.Add(new { inline_data = new { mime_type = "image/jpeg", data = base64 } });
            }
        }
    }
}