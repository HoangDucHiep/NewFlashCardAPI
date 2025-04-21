using System.Security.Claims;
using FlashcardApi.Application.Folder;
using FlashcardApi.Application.Folder.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardApi.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FolderController : ControllerBase
{
    private readonly IFolderService _folderService;

    public FolderController(IFolderService folderService)
    {
        _folderService = folderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserFolders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var folders = await _folderService.GetUserFoldersAsync(userId);
        return Ok(folders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder([FromBody] FolderDto folderDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var folder = await _folderService.CreateFolderAsync(userId, folderDto);
        return Ok(folder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFolder(string id, [FromBody] FolderDto folderDto)
    {
        try
        {
            var updatedFolder = await _folderService.UpdateFolderAsync(id, folderDto);
            return Ok(updatedFolder);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFolder(string id)
    {
        var deleted = await _folderService.DeleteFolderAsync(id);
        return deleted ? Ok(new { message = "Folder deleted successfully" }) : NotFound(new { message = "Folder not found" });
    }

    [HttpGet("nested")]
    public async Task<IActionResult> GetNestedFolders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nestedFolders = await _folderService.GetNestedFoldersAsync(userId);
        return Ok(nestedFolders);
    }
}