using System.ComponentModel.DataAnnotations;

namespace MyWebApplication.Models;

public enum TranslationType
{
    LeetSpeak = 1,
    Fake = 2
}

public class Translation
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500, ErrorMessage = "Too long text to translate")]
    public string Input { get; set; }

    [MaxLength(1000)]
    public string Output { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TranslationType Type { get; set; }

    [Required]
    public bool IsSuccess { get; set; }

    public string ErrorMessage { get; set; }

}