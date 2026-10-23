
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShopAI.Data;

namespace SmartShopAI.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;

        public ReportsController(SmartShopDbContext context)
        {
            _context = context;
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviewReport()
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync();

            var reviews = await _context.Reviews
                .AsNoTracking()
                .ToListAsync();

            var report = products.Select(p =>
            {
                var productReviews = reviews
                    .Where(r => r.ProductId == p.ProductId)
                    .ToList();

                return new
                {
                    productId = p.ProductId,
                    productName = p.ProductName,

                    totalReviews = productReviews.Count,

                    averageRating = productReviews
                        .Where(r => r.Rating.HasValue)
                        .Select(r => (double?)r.Rating!.Value)
                        .Average() ?? 0,

                    positive = productReviews.Count(r =>
                        r.Sentiment == "Positive"),

                    neutral = productReviews.Count(r =>
                        r.Sentiment == "Neutral"),

                    negative = productReviews.Count(r =>
                        r.Sentiment == "Negative"),

                    averageSentimentScore = productReviews
                        .Where(r => r.Score.HasValue)
                        .Select(r => r.Score)
                        .Average() ?? 0
                };
            }).ToList();

            return Ok(report);
        }
    }
}
