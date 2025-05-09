using System.Security.Claims;
using FlashcardApi.Application.Desk;
using FlashcardApi.Application.Desk.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardApi.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DeskController : ControllerBase
{
    private readonly IDeskService _deskService;

    public DeskController(IDeskService deskService)
    {
        _deskService = deskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserDesks()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var desks = await _deskService.GetUserDesksAsync(userId);
        return Ok(desks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeskById(string id)
    {
        var desk = await _deskService.GetDeskByIdAsync(id);
        return desk == null ? NotFound(new { message = "Desk not found" }) : Ok(desk);
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicDesks()
    {
        var desks = await _deskService.GetPublicDesksAsync();
        return Ok(desks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDesk([FromBody] DeskDto deskDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var desk = await _deskService.CreateDeskAsync(userId, deskDto);
        return Ok(desk);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDesk(string id, [FromBody] DeskDto deskDto)
    {
        try
        {
            var updatedDesk = await _deskService.UpdateDeskAsync(id, deskDto);
            return Ok(updatedDesk);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDesk(string id)
    {
        var deleted = await _deskService.DeleteDeskAsync(id);
        return deleted ? Ok(new { message = "Desk deleted successfully" }) : NotFound(new { message = "Desk not found" });
    }

    [HttpPost("{id}/clone")]
    public async Task<IActionResult> CloneDesk(string id, [FromQuery] string targetFolderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        try
        {
            var clonedDesk = await _deskService.CloneDeskAsync(userId, id, targetFolderId);
            return Ok(clonedDesk);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}