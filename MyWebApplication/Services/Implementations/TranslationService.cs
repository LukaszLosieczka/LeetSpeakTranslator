using System.Reflection.Metadata;
using MyWebApplication.Data;
using MyWebApplication.Exceptions;
using MyWebApplication.Models;

namespace MyWebApplication.Services.Implementations;

public class TranslationService : ITranslationService
{
    private const int MAX_LENGTH = 500;
    private readonly IEnumerable<ITranslationStrategy> _strategies;
    private readonly DatabaseContext _dbContext;

    public TranslationService(IEnumerable<ITranslationStrategy> strategies, DatabaseContext dbContext)
    {
        _strategies = strategies;
        _dbContext = dbContext;
    }

    public async Task<string> Translate(string input, TranslationType translationType)
    {
        if (input.Length > MAX_LENGTH)
        {
            throw new TranslationException("Text to translate is too long.");
        }
        var strategy = GetTranslationStrategy(translationType);
        if (strategy == null)
        {
            throw new TranslationException($"No translation strategy found for type {translationType}");
        }

        var translation = new Translation
        {
            Input = input,
            Date = DateTime.UtcNow,
            Type = translationType,
        };
        try
        {
            var result = await strategy.Translate(input);
            translation.Output = result;
            translation.IsSuccess = true;
            _dbContext.Translations.Add(translation);
            await _dbContext.SaveChangesAsync();
            return result;
        }
        catch (TranslationException ex)
        {
            translation.ErrorMessage = ex.Message;
            translation.IsSuccess = false;
            _dbContext.Translations.Add(translation);
            await _dbContext.SaveChangesAsync();
            throw;
        }
    }

    private ITranslationStrategy GetTranslationStrategy(TranslationType type)
    {
        return _strategies.FirstOrDefault(strategy => strategy.SupportsTranslationType(type));
    }
}