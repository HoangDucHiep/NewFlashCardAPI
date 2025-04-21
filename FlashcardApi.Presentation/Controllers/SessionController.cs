using FlashcardApi.Application.Interfaces;
using FlashcardApi.Application.Session.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardApi.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] SessionDto sessionDto)
    {
        var session = await _sessionService.CreateSessionAsync(sessionDto);
        return Ok(session);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSession(string id, [FromBody] SessionDto sessionDto)
    {
        try
        {
            var updatedSession = await _sessionService.UpdateSessionAsync(id, sessionDto);
            return Ok(updatedSession);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(string id)
    {
        var deleted = await _sessionService.DeleteSessionAsync(id);
        return deleted ? Ok(new { message = "Session deleted successfully" }) : NotFound(new { message = "Session not found" });
    }

    [HttpGet("desk/{deskId}")]
    public async Task<IActionResult> GetSessionsByDeskId(string deskId)
    {
        var sessions = await _sessionService.GetSessionsByDeskIdAsync(deskId);
        return Ok(sessions);
    }
}