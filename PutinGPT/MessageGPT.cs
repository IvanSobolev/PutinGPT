using System.Text.Json.Serialization;

namespace PutinGPT;

public class MessageGPT(string role, string content)
{
    [JsonPropertyName("role")]
    public  string Role { get; set; } = role;
    
    [JsonPropertyName("content")]
    public  string Content { get; set; } = content;
}