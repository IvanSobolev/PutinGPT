using System.Text.Json.Serialization;

namespace PutinGPT;

public class StatusResponse(string status, string answer)
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = status;
    
    [JsonPropertyName("gpt")]
    public string Answer { get; set; } = answer;
}