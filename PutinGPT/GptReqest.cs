using System.Text;
using System.Text.Json;

namespace PutinGPT;

public static class GptReqest
{
    private static readonly string UrlSendMessage = "https://nexra.aryahcr.cc/api/chat/gpt";
    private static readonly string UrlGetMessage = "https://nexra.aryahcr.cc/api/chat/task/";

    public static async Task<IdResponse?> SendMessage(ChatGPT chatBody)
    {
        using HttpClient client = new HttpClient();

        var jsonContent = JsonSerializer.Serialize(chatBody);
        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(UrlSendMessage, content);
            
            try
            {
                IdResponse? obj = JsonSerializer.Deserialize<IdResponse>(await response.Body());
                return obj;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialize error: {ex.Message}");
                return default;
            }
            
        }
        catch  (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return default;
        }
    }
    
    public static async Task<StatusResponse?> GetMessage(string id)
    {
        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(UrlGetMessage + id);
            
            try
            {
                StatusResponse? obj = JsonSerializer.Deserialize<StatusResponse>(await response.Body());
                return obj;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialize error: {ex.Message}");
                return default;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return default;
        }
    }
    
    static async Task<string> Body(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}