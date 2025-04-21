using System.Security.Claims;
using FlashcardApi.Application.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardApi.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(new { message = "No file uploaded" });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var image = await _imageService.UploadImageAsync(userId, file);
        return Ok(image);
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteImage(string fileName)
    {
        var deleted = await _imageService.DeleteImageAsync(fileName);
        return deleted ? Ok(new { message = "Image deleted successfully" }) : NotFound(new { message = "Image not found" });
    }
}