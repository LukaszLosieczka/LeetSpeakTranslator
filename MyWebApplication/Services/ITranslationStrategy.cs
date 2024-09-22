using MyWebApplication.Models;

namespace MyWebApplication.Services;

public interface ITranslationStrategy
{
    Task<string> Translate(string input);

    bool SupportsTranslationType(TranslationType type);
}