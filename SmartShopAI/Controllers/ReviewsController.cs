
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShopAI.Data;
using SmartShopAI.DTOs;
using SmartShopAI.Models;
using SmartShopAI.Services;
using System.Net;
using System.Text.Json;

namespace SmartShopAI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly GeminiService _geminiService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            SmartShopDbContext context,
            GeminiService geminiService,
            ILogger<ReviewsController> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze(
            [FromBody] AnalyzeReviewRequest request)
        {
            // 1. Kiểm tra sản phẩm
            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == request.ProductId);

            if (!productExists)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            // 2. Tạo prompt phân tích đánh giá
            var prompt = $$"""
                You are a customer review sentiment analyst.

                Analyze the following customer review.

                Review: {{request.Content}}
                Rating: {{request.Rating}}/5

                Return ONLY valid JSON:
                {
                  "sentiment": "Positive",
                  "score": 0.92,
                  "summary": "Short review summary"
                }

                Rules:
                - sentiment must be Positive, Negative, or Neutral.
                - score must be between 0 and 1.
                - summary must be 256 characters or fewer.
                - Consider both review content and rating.
                - Do not include markdown or explanations.
                """;

            string aiResponse;

            // 3. Gọi Gemini AI
            try
            {
                aiResponse =
                    await _geminiService.GenerateAsync(prompt);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(
                    ex, "Gemini API request failed.");

                if (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return StatusCode(429, new
                    {
                        message = "Gemini API rate limit exceeded."
                    });
                }

                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return StatusCode(502, new
                    {
                        message = "Gemini model is unavailable or invalid."
                    });
                }

                return StatusCode(503, new
                {
                    message = "Gemini AI is temporarily unavailable."
                });
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(
                    ex, "Gemini request timed out.");

                return StatusCode(504, new
                {
                    message = "Gemini API request timed out."
                });
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex, "Gemini returned an invalid API response.");

                return StatusCode(502, new
                {
                    message = "Invalid Gemini API response."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex, "Gemini response could not be processed.");

                return StatusCode(502, new
                {
                    message = "Gemini response could not be processed."
                });
            }

            // 4. Phân tích JSON do Gemini trả về
            string sentiment;
            double score;
            string summary;

            try
            {
                var cleanedJson = aiResponse.Trim();

                if (cleanedJson.StartsWith("```"))
                {
                    cleanedJson = cleanedJson
                        .Replace("```json", "")
                        .Replace("```JSON", "")
                        .Replace("```", "")
                        .Trim();
                }

                using var json =
                    JsonDocument.Parse(cleanedJson);

                var root = json.RootElement;

                sentiment = root
                    .GetProperty("sentiment")
                    .GetString() ?? "";

                score = root
                    .GetProperty("score")
                    .GetDouble();

                summary = root
                    .GetProperty("summary")
                    .GetString() ?? "";
            }
            catch (Exception ex) when (
                ex is JsonException ||
                ex is KeyNotFoundException ||
                ex is InvalidOperationException)
            {
                _logger.LogWarning(
                    ex, "Invalid review analysis JSON.");

                return StatusCode(502, new
                {
                    message = "Gemini returned invalid analysis JSON."
                });
            }

            // 5. Kiểm tra dữ liệu phân tích
            if (sentiment is not
                    ("Positive" or "Negative" or "Neutral")
                || double.IsNaN(score)
                || double.IsInfinity(score)
                || score < 0
                || score > 1
                || string.IsNullOrWhiteSpace(summary)
                || summary.Length > 256)
            {
                return StatusCode(502, new
                {
                    message = "Gemini returned invalid analysis values."
                });
            }

            // 6. Lưu kết quả vào SQL Server
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

            try
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex, "Failed to save review.");

                return StatusCode(500, new
                {
                    message = "Failed to save review to database."
                });
            }

            // 7. Trả kết quả
            return Ok(new
            {
                review.ReviewId,
                review.ProductId,
                review.CustomerName,
                review.Content,
                review.Rating,
                review.Sentiment,
                review.Score,
                review.Summary,
                review.CreatedAt
            });
        }
    }
}
