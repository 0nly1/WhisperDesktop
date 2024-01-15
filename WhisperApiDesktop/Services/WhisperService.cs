using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace WhisperApiDesktop.Services;

public class WhisperService
{
    private readonly string _openAiToken;

    private const string WhisperLink = "https://api.openai.com/v1/audio";

    public static string[] ResponseFormats =
    {
        "text", "srt", "json", "verbose_json", "vtt"
    };
    
    public async Task<string> GetTranscriptions(string path, string? prompt = null, 
        string format = "text", string langCode = "ru", double temperature = 0)
    {
        var client = new RestClient(WhisperLink);
        var req = new RestRequest("/transcriptions", Method.Post)
            .AddHeader("Authorization", $"Bearer {_openAiToken}")
            .AddHeader("Content-Type", "multipart/form-data")
            .AddParameter("model", "whisper-1")
            .AddParameter("response_format", format)
            .AddParameter("language", langCode)
            .AddParameter("temperature", temperature.ToString(CultureInfo.InvariantCulture))
            .AddFile("file", path);

        if (!string.IsNullOrEmpty(prompt))
            req.AddParameter("prompt", prompt);
        
        var response = await client.ExecuteAsync(req);

        if (string.IsNullOrEmpty(response.Content))
            return string.Empty;

        string text;
        
        if (format == "text" || format == "srt" || format == "vtt")
        {
            text = response.Content;
            return string.IsNullOrEmpty(text) ? string.Empty : text;
        }
        
        var doc = JsonDocument.Parse(response.Content);
        text = doc.RootElement.GetProperty("text").GetString()!;

        return string.IsNullOrEmpty(text) ? string.Empty : text;
    }

    public WhisperService(string openAiToken)
    {
        _openAiToken = openAiToken;
    }
}