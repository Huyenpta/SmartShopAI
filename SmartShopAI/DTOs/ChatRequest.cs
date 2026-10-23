
using System.ComponentModel.DataAnnotations;

namespace SmartShopAI.DTOs
{
    public class ChatRequest
    {
        [Required]
        public string SessionId { get; set; } = "";

        [Required]
        public string Message { get; set; } = "";
    }
}
