namespace MyWebApplication.Dto;

public class TranslationResponseDto
{
    public bool IsSuccess { get; set; }
    public string TranslatedText { get; set; }
    public string ErrorMessage { get; set; }
}