using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using MyWebApplication.Dto.External;
using MyWebApplication.Exceptions;
using MyWebApplication.Models;
using MyWebApplication.Services.Implementations.TranslationStrategies;

namespace MyWebApplication.Tests.Services;

public class LeetSpeakTranslationTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly LeetSpeakTranslation _leetSpeakTranslationUnderTest;

    public LeetSpeakTranslationTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _leetSpeakTranslationUnderTest = new LeetSpeakTranslation(_httpClient);
    }

    [Fact]
    public async Task Translate_ReturnsTranslatedText_WhenApiCallIsSuccessful()
    {
        // given
        var input = "Hello";
        var expectedTranslatedText = "H3ll0";
        var apiResponse = new FunTranslationsApiResponse
        {
            contents = new Contents { translated = expectedTranslatedText }
        };

        var responseContent = JsonSerializer.Serialize(apiResponse);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // when
        var result = await _leetSpeakTranslationUnderTest.Translate(input);

        // then
        Assert.Equal(expectedTranslatedText, result);
    }

    [Fact]
    public async Task Translate_ThrowsTranslationException_WhenApiReturnsError()
    {
        // given
        var input = "Hello";
        var errorMessage = "Error occurred";
        var apiError = new FunTranslationsApiError
        {
            error = new Error { message = errorMessage }
        };

        var responseContent = JsonSerializer.Serialize(apiError);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // when then
        var exception = await Assert.ThrowsAsync<TranslationException>(() => _leetSpeakTranslationUnderTest.Translate(input));
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public void SupportsTranslationType_ReturnsTrue_ForLeetSpeakTranslationType()
    {
        // given
        var translationType = TranslationType.LeetSpeak;

        // when
        var result = _leetSpeakTranslationUnderTest.SupportsTranslationType(translationType);

        // then
        Assert.True(result);
    }

    [Fact]
    public void SupportsTranslationType_ReturnsFalse_ForNonLeetSpeakTranslationType()
    {
        // given
        var translationType = TranslationType.Fake;

        // when
        var result = _leetSpeakTranslationUnderTest.SupportsTranslationType(translationType);

        // then
        Assert.False(result);
    }
}