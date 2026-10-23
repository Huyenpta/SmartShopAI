
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartShopAI.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public int ProductId { get; set; }

        [MaxLength(64)]
        public string? CustomerName { get; set; }

        [MaxLength(1000)]
        public string? Content { get; set; }

        public int? Rating { get; set; }

        [MaxLength(16)]
        public string? Sentiment { get; set; }

        public double? Score { get; set; }

        [MaxLength(256)]
        public string? Summary { get; set; }

        public DateTime? CreatedAt { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
