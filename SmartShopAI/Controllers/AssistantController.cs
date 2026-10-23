
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShopAI.Data;
using SmartShopAI.DTOs;
using SmartShopAI.Services;

namespace SmartShopAI.Controllers
{
    [ApiController]
    [Route("api/assistant")]
    public class AssistantController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly GeminiService _gemini;

        public AssistantController(
            SmartShopDbContext context,
            GeminiService gemini)
        {
            _context = context;
            _gemini = gemini;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat(
            [FromBody] ChatRequest request)
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync();

            var productInfo = string.Join("\n",
                products.Select(p =>
                    $"ID: {p.ProductId}, " +
                    $"Name: {p.ProductName}, " +
                    $"Category: {p.Category}, " +
                    $"Price: {p.Price}, " +
                    $"Features: {p.Features}, " +
                    $"Description: {p.Description}"));

            var prompt = $"""
                You are a shopping assistant for SmartShopAI.

                Answer the customer's question using
                ONLY the product information below.

                PRODUCTS:
                {productInfo}

                CUSTOMER QUESTION:
                {request.Message}

                Rules:
                - Answer in the same language as the customer.
                - Do not invent products or prices.
                - If information is unavailable, say so.
                - Be friendly and concise.
                """;

            try
            {
                var answer = await _gemini.GenerateAsync(prompt);

                return Ok(new
                {
                    sessionId = request.SessionId,
                    answer
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new
                {
                    message = "Gemini is temporarily unavailable."
                });
            }
        }
    }
}
