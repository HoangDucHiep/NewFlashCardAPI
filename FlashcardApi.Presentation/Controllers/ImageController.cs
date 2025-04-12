using System.Security.Claims;
using System.Threading.Tasks;
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
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string fileName)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });
        if (string.IsNullOrEmpty(fileName))
            return BadRequest(new { message = "File name is required" });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var image = await _imageService.UploadImageAsync(userId, file, fileName);
        return Ok(image);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage([FromQuery] string fileName)
    {
        try
        {
            await _imageService.DeleteImageAsync(fileName);
            return Ok(new { message = "Image deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
