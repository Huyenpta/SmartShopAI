
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartShopAI.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
            _configuration = configuration;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException(
                    "Prompt không được để trống.");

            var apiKey = _configuration["Gemini:ApiKey"];

            var model = _configuration["Gemini:Model"]
            ?? "gemini-3.5-flash-lite";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Chưa cấu hình Gemini API Key.");

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Post, url);

                request.Headers.Add("x-goog-api-key", apiKey);
                request.Content = JsonContent.Create(requestBody);

                using var response =
                    await _httpClient.SendAsync(request);

                var responseText =
                    await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var json =
                        JsonDocument.Parse(responseText);

                    if (!json.RootElement.TryGetProperty(
                            "candidates", out var candidates)
                        || candidates.GetArrayLength() == 0)
                    {
                        throw new InvalidOperationException(
                            "Gemini không trả về kết quả.");
                    }

                    var content = candidates[0]
                        .GetProperty("content");

                    var parts = content.GetProperty("parts");

                    foreach (var part in parts.EnumerateArray())
                    {
                        if (part.TryGetProperty(
                                "text", out var textElement))
                        {
                            var result = textElement.GetString();

                            if (!string.IsNullOrWhiteSpace(result))
                                return result;
                        }
                    }

                    throw new InvalidOperationException(
                        "Gemini không trả về nội dung văn bản.");
                }

                bool retryable =
                    response.StatusCode ==
                        HttpStatusCode.ServiceUnavailable
                    || response.StatusCode ==
                        HttpStatusCode.TooManyRequests
                    || response.StatusCode ==
                        HttpStatusCode.BadGateway
                    || response.StatusCode ==
                        HttpStatusCode.GatewayTimeout;

                if (retryable && attempt < maxAttempts)
                {
                    int delaySeconds =
                        attempt == 1 ? 2 : 5;

                    await Task.Delay(
                        TimeSpan.FromSeconds(delaySeconds));

                    continue;
                }

                throw new HttpRequestException(
                    $"Gemini API error {(int)response.StatusCode}: {responseText}",
                    null,
                    response.StatusCode);
            }

            throw new InvalidOperationException(
                "Không thể nhận phản hồi từ Gemini.");
        }
    }
}
