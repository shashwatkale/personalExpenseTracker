using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ExpenseTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ExpenseTracker.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _aiServiceUrl;

    public AiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _aiServiceUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
    }

    public async Task<string> GetSpendingSummaryAsync(Guid userId, Dictionary<string, decimal> categoryBreakdown, decimal total)
    {
        var request = new
        {
            user_id = userId.ToString(),
            total_amount = total,
            category_breakdown = categoryBreakdown
        };

        var response = await _httpClient.PostAsync(
            $"{_aiServiceUrl}/ai/summary",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
            return "Unable to generate AI summary at this time.";

        var result = await response.Content.ReadFromJsonAsync<AiSummaryResponse>();
        return result?.Summary ?? "No summary available.";
    }

    public async Task<List<string>> GetSavingSuggestionsAsync(Guid userId, Dictionary<string, decimal> categoryBreakdown, decimal total)
    {
        var request = new
        {
            user_id = userId.ToString(),
            total_amount = total,
            category_breakdown = categoryBreakdown
        };

        var response = await _httpClient.PostAsync(
            $"{_aiServiceUrl}/ai/suggestions",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
            return new List<string> { "Unable to generate suggestions at this time." };

        var result = await response.Content.ReadFromJsonAsync<AiSuggestionsResponse>();
        return result?.Suggestions ?? new List<string>();
    }

    private record AiSummaryResponse(string Summary);
    private record AiSuggestionsResponse(List<string> Suggestions);
}
