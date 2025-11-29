// Controllers/GeminiSpaController.cs
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class GeminiSpaController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _jsonData;

    public GeminiSpaController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

        // Đọc services.json từ wwwroot/data
        var basePath = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(basePath, "wwwroot", "data", "services.json");
        if (!System.IO.File.Exists(filePath))
            throw new FileNotFoundException("Không tìm thấy services.json", filePath);

        _jsonData = System.IO.File.ReadAllText(filePath);
    }

    [HttpPost("consult")]
    public async Task<IActionResult> Consult([FromBody] ConsultRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("Câu hỏi không được để trống.");

        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return StatusCode(500, "Chưa cấu hình Gemini API Key.");

        var prompt = $"""
        Bạn là tư vấn viên spa chuyên nghiệp, thân thiện, nói tiếng Việt tự nhiên.

        DỮ LIỆU DỊCH VỤ SPA (JSON):
        {_jsonData}

        CÂU HỎI CỦA KHÁCH:
        {request.Question}

        NHIỆM VỤ:
        - Phân tích nhu cầu.
        - Gợi ý 1-2 gói phù hợp nhất.
        - Trả lời ngắn gọn: **Tên gói**, giá, thời gian, lý do.
        - Kết thúc bằng: "Bạn muốn đặt lịch không ạ?"
        - Không bịa thông tin.

        Trả lời bằng tiếng Việt, dùng **bold** cho tên gói.
        """;

        var payload = new
        {
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = prompt } } }
            },
            generationConfig = new { temperature = 0.7, maxOutputTokens = 250 }
        };

        var client = _httpClientFactory.CreateClient();
        var jsonContent = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={apiKey}";

        try
        {
            var response = await client.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode(500, $"Lỗi Gemini API: {responseJson}");

            using var doc = JsonDocument.Parse(responseJson);
            var answer = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "Xin lỗi, chưa tìm thấy dịch vụ phù hợp.";

            return Ok(new { answer });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }
}

public class ConsultRequest
{
    public string Question { get; set; } = "";
}