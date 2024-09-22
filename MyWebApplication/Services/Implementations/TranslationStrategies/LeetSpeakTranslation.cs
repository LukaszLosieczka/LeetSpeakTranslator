using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using MyWebApplication.Dto.External;
using MyWebApplication.Exceptions;
using MyWebApplication.Models;

namespace MyWebApplication.Services.Implementations.TranslationStrategies;

public class LeetSpeakTranslation : ITranslationStrategy
{
    private const string URL = "https://api.funtranslations.com/translate/leetspeak.json";
    private readonly HttpClient _httpClient;

    public LeetSpeakTranslation(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool SupportsTranslationType(TranslationType type)
    {
        return type.Equals(TranslationType.LeetSpeak);
    }

    public async Task<string> Translate(string input)
    {
        var requestBody = new { text = input };
        var content = new StringContent(JsonSerializer.Serialize(requestBody));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await _httpClient.PostAsync(URL, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK){
            var translationError = JsonSerializer.Deserialize<FunTranslationsApiError>(jsonResponse);
            throw new TranslationException(translationError.error.message);
        }
        var translationResult = JsonSerializer.Deserialize<FunTranslationsApiResponse>(jsonResponse);
        return translationResult.contents.translated;
    }
}