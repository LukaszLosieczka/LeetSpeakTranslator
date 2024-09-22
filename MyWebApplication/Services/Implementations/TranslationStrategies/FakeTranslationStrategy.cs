using MyWebApplication.Models;

namespace MyWebApplication.Services.Implementations.TranslationStrategies;

public class FakeTranslationStrategy : ITranslationStrategy
{
    public bool SupportsTranslationType(TranslationType type)
    {
        return type.Equals(TranslationType.Fake);
    }

    public async Task<string> Translate(string input)
    {
        await Task.Delay(1000);
        return "This is a fake translation strategy";
    }
}