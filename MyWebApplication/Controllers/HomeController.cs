using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApplication.Dto;
using MyWebApplication.Exceptions;
using MyWebApplication.Models;
using MyWebApplication.Services;
using MyWebApplication.ViewModels;

namespace MyWebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ITranslationService _translationService;

    public HomeController(ILogger<HomeController> logger, ITranslationService translationService)
    {
        _logger = logger;
        _translationService = translationService;
    }

    public IActionResult Index()
    {
        var model = new TranslateViewModel
        {
            AvailableTypes = getAvailableTypes()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> Translate(TranslationRequestDto request)
    {
        var response = new TranslationResponseDto();
        try
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.TextToTranslate))
            {
                throw new TranslationException("Incorrect request");
            }
            response.TranslatedText = await _translationService.Translate(request.TextToTranslate, request.Type);
            response.IsSuccess = true;
        }
        catch (TranslationException ex)
        {
            Console.WriteLine(ex.Message);
            response.ErrorMessage = ex.Message;
            response.IsSuccess = false;
        }
        return Json(response);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private List<SelectListItem> getAvailableTypes()
    {
        return Enum.GetValues(typeof(TranslationType))
                              .Cast<TranslationType>()
                              .Select(type => new SelectListItem
                              {
                                  Value = ((int)type).ToString(),
                                  Text = type.ToString()
                              }).ToList();
    }
}
