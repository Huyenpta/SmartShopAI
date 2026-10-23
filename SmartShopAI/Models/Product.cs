
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartShopAI.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [MaxLength(128)]
        public string? ProductName { get; set; }

        [MaxLength(64)]
        public string? Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [MaxLength(512)]
        public string? Features { get; set; }

        public string? Description { get; set; }
    }
}
