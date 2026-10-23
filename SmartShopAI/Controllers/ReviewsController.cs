
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShopAI.Data;
using SmartShopAI.DTOs;
using SmartShopAI.Models;
using SmartShopAI.Services;
using System.Text.Json;

namespace SmartShopAI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly GeminiService _geminiService;

        public ReviewsController(
            SmartShopDbContext context,
            GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze(
            [FromBody] AnalyzeReviewRequest request)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == request.ProductId);

            if (!productExists)
                return NotFound("Product not found.");


            var prompt = $$"""
            Analyze the following customer review.

            Review: {{request.Content}}
            Rating: {{request.Rating}}/5

            Return ONLY valid JSON with this structure:
            {
              "sentiment": "Positive",
              "score": 0.92,
              "summary": "Short review summary"
            }

            Rules:
            - sentiment must be Positive, Negative, or Neutral.
            - score must be a number from 0 to 1.
            - summary must be 256 characters or fewer.
            - Do not include markdown or explanations.
            """;


            var aiResponse = await _geminiService.GenerateAsync(prompt);

            try
            {
                var cleanedJson = aiResponse.Trim();

                if (cleanedJson.StartsWith("```"))
                {
                    cleanedJson = cleanedJson
                        .Replace("```json", "")
                        .Replace("```", "")
                        .Trim();
                }

                using var json = JsonDocument.Parse(cleanedJson);
                var root = json.RootElement;

                var sentiment = root.GetProperty("sentiment").GetString();
                var score = root.GetProperty("score").GetDouble();
                var summary = root.GetProperty("summary").GetString();

                if (sentiment is not ("Positive" or "Negative" or "Neutral")
                    || score < 0 || score > 1
                    || string.IsNullOrWhiteSpace(summary)
                    || summary.Length > 256)
                {
                    return StatusCode(502,
                        "Gemini returned invalid analysis values.");
                }

                var review = new Review
                {
                    ProductId = request.ProductId,
                    CustomerName = request.CustomerName,
                    Content = request.Content,
                    Rating = request.Rating,
                    Sentiment = sentiment,
                    Score = score,
                    Summary = summary,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    review.ReviewId,
                    review.ProductId,
                    review.Sentiment,
                    review.Score,
                    review.Summary
                });
            }
            catch (JsonException)
            {
                return StatusCode(502,
                    "Gemini returned invalid JSON.");
            }
        }
    }
}
