using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyWebApplication.ViewModels;

public class TranslateViewModel
{
    public List<SelectListItem> AvailableTypes { get; set; }
}