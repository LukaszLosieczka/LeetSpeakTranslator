using MyWebApplication.Models;

namespace MyWebApplication.Services;

public interface ITranslationService
{
    Task<string> Translate(string input, TranslationType translationType);
}