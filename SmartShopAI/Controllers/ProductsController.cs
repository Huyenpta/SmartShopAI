
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShopAI.Data;
using SmartShopAI.Services;

namespace SmartShopAI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly GeminiService _gemini;

        public ProductsController(
            SmartShopDbContext context,
            GeminiService gemini)
        {
            _context = context;
            _gemini = gemini;
        }

        [HttpPost("{id}/generatedescription")]
        public async Task<IActionResult> GenerateDescription(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound("Product not found.");

            var prompt = $"""
                You are a professional marketing copywriter.

                Write an attractive product description
                of approximately 80 words in English.

                Product name: {product.ProductName}
                Category: {product.Category}
                Price: {product.Price} VND
                Features: {product.Features}

                Return only the product description.
                Do not invent product specifications.
                """;

            var description = await _gemini.GenerateAsync(prompt);

            product.Description = description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                productId = product.ProductId,
                productName = product.ProductName,
                description = product.Description
            });
        }
    }
}
