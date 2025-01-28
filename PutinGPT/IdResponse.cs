using System.Text.Json.Serialization;

namespace PutinGPT;

public class IdResponse(string id, string status)
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = status;
}