using Microsoft.EntityFrameworkCore;
using Moq;
using MyWebApplication.Data;
using MyWebApplication.Exceptions;
using MyWebApplication.Models;
using MyWebApplication.Services;
using MyWebApplication.Services.Implementations;

namespace MyWebApplication.Tests.Services;

public class TranslationServiceTests : IDisposable
{
    private readonly Mock<ITranslationStrategy> _mockStrategy;
    private readonly DatabaseContext _dbContext;
    private readonly List<ITranslationStrategy> _strategies;
    private readonly TranslationService _serviceUnderTest;

    public TranslationServiceTests()
    {
        _mockStrategy = new Mock<ITranslationStrategy>();
        _strategies = new List<ITranslationStrategy> { _mockStrategy.Object };

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new DatabaseContext(options);

        _serviceUnderTest = new TranslationService(_strategies, _dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task Translate_ThrowsException_WhenInputTextTooLong()
    {
        // given
        var longText = new string('a', 501);
        var translationType = TranslationType.LeetSpeak;

        // when & then
        await Assert.ThrowsAsync<TranslationException>(() => _serviceUnderTest.Translate(longText, translationType));
    }

    [Fact]
    public async Task Translate_ThrowsException_WhenNoStrategyFound()
    {
        // given
        var input = "Hello";
        var translationType = TranslationType.LeetSpeak;

        _mockStrategy.Setup(s => s.SupportsTranslationType(translationType)).Returns(false);

        // when then
        await Assert.ThrowsAsync<TranslationException>(() => _serviceUnderTest.Translate(input, translationType));
    }

    [Fact]
    public async Task Translate_Successfully_TranslatesText_And_SavesToDatabase()
    {
        // given
        var input = "Hello";
        var translationType = TranslationType.LeetSpeak;
        var expectedOutput = "H3ll0";

        _mockStrategy.Setup(s => s.SupportsTranslationType(translationType)).Returns(true);
        _mockStrategy.Setup(s => s.Translate(input)).ReturnsAsync(expectedOutput);

        // when
        var result = await _serviceUnderTest.Translate(input, translationType);

        // then
        Assert.Equal(expectedOutput, result);

        var translationInDb = await _dbContext.Translations.FirstOrDefaultAsync();
        Assert.NotNull(translationInDb);
        Assert.Equal(input, translationInDb.Input);
        Assert.Equal(expectedOutput, translationInDb.Output);
        Assert.True(translationInDb.IsSuccess);
        Assert.Equal(translationType, translationInDb.Type);
    }

    [Fact]
    public async Task Translate_Fails_And_SavesErrorToDatabase()
    {
        // given
        var input = "Hello";
        var translationType = TranslationType.LeetSpeak;
        var errorMessage = "Translation error";

        _mockStrategy.Setup(s => s.SupportsTranslationType(translationType)).Returns(true);
        _mockStrategy.Setup(s => s.Translate(input)).ThrowsAsync(new TranslationException(errorMessage));

        // when then
        var exception = await Assert.ThrowsAsync<TranslationException>(() => _serviceUnderTest.Translate(input, translationType));

        Assert.Equal(errorMessage, exception.Message);

        var translationInDb = await _dbContext.Translations.FirstOrDefaultAsync();
        Assert.NotNull(translationInDb);
        Assert.Equal(input, translationInDb.Input);
        Assert.Equal(errorMessage, translationInDb.ErrorMessage);
        Assert.False(translationInDb.IsSuccess);
        Assert.Equal(translationType, translationInDb.Type);
    }
}