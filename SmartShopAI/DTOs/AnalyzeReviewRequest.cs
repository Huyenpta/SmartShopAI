
using System.ComponentModel.DataAnnotations;

namespace SmartShopAI.DTOs
{
    public class AnalyzeReviewRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(64)]
        public string CustomerName { get; set; } = "";

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = "";

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
