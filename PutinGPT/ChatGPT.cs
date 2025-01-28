using System.Text.Json.Serialization;

namespace PutinGPT;

public class ChatGPT(List<MessageGPT> messages, string promt)
{
    [JsonPropertyName("messages")]
    public List<MessageGPT> Messages { get; set; } = messages;
    
    [JsonPropertyName("promt")]
    public string Promt { get; set; } = promt;
    
    [JsonPropertyName("model")]
    public static string Model { get; set; } = "GPT-4";
    
    [JsonPropertyName("markdown")]
    public static bool Markdown { get; set; } = true;
}