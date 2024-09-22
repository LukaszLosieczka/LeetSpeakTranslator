namespace MyWebApplication.Dto.External;

public class FunTranslationsApiError
{
    public Error error { get; set; }
}

public class Error
{
    public int code { get; set; }
    public string message { get; set; }
}