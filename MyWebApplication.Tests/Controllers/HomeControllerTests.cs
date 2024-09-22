using Moq;
using MyWebApplication.Controllers;
using MyWebApplication.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Dto;
using MyWebApplication.ViewModels;
using MyWebApplication.Models;
using MyWebApplication.Exceptions;

namespace MyWebApplication.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly Mock<ITranslationService> _mockTranslationService;
    private readonly HomeController _controllerUnderTest;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _mockTranslationService = new Mock<ITranslationService>();

        _controllerUnderTest = new HomeController(_mockLogger.Object, _mockTranslationService.Object);
    }


    [Fact]
    public void Index_Returns_ViewResult_With_TranslateViewModel()
    {
        // when
        var result = _controllerUnderTest.Index();

        // then
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TranslateViewModel>(viewResult.Model);
        Assert.NotNull(model.AvailableTypes);
    }

    [Fact]
    public async Task Translate_Returns_JsonResult_With_Success_Response()
    {
        // given
        var input = "Hello";
        var expectedOutput = "H3llo";
        var request = new TranslationRequestDto
        {
            TextToTranslate = input,
            Type = TranslationType.LeetSpeak
        };

        _mockTranslationService
            .Setup(service => service.Translate(It.IsAny<string>(), It.IsAny<TranslationType>()))
            .ReturnsAsync(expectedOutput);

        // when 
        var result = await _controllerUnderTest.Translate(request);

        // then
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<TranslationResponseDto>(jsonResult.Value);
        Assert.True(response.IsSuccess);
        Assert.Equal(expectedOutput, response.TranslatedText);
    }

    [Fact]
    public async Task Translate_Returns_JsonResult_With_Error_On_EmptyText()
    {
        // given
        var request = new TranslationRequestDto
        {
            TextToTranslate = "",
            Type = TranslationType.LeetSpeak
        };

        // when
        var result = await _controllerUnderTest.Translate(request);

        // then
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<TranslationResponseDto>(jsonResult.Value);
        Assert.False(response.IsSuccess);
        Assert.Null(response.TranslatedText);
        Assert.NotNull(response.ErrorMessage);
    }

    [Fact]
    public async Task Translate_Returns_JsonResult_With_Error_On_TranslationService_Exception()
    {
        // given
        var errorMessage = "Error during translation";
        var request = new TranslationRequestDto
        {
            TextToTranslate = "Hello",
            Type = TranslationType.LeetSpeak
        };

        _mockTranslationService
            .Setup(service => service.Translate(It.IsAny<string>(), It.IsAny<TranslationType>()))
            .ThrowsAsync(new TranslationException(errorMessage));

        // when
        var result = await _controllerUnderTest.Translate(request);

        // then
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<TranslationResponseDto>(jsonResult.Value);
        Assert.False(response.IsSuccess);
        Assert.Equal(errorMessage, response.ErrorMessage);
    }
}