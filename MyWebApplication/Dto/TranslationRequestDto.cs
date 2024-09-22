using MyWebApplication.Models;

namespace MyWebApplication.Dto;

public class TranslationRequestDto
{
    public string TextToTranslate { get; set; }
    public TranslationType Type { get; set; }
}