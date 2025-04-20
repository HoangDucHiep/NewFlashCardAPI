using FlashcardApi.Application.Desk.Dtos;

namespace FlashcardApi.Application.Desk;

public interface IDeskService
{
    Task<List<DeskDto>> GetUserDesksAsync(string userId);
    Task<List<PublicDeskDto>> GetPublicDesksAsync();
    Task<DeskDto> CreateDeskAsync(string userId, DeskDto deskDto);
    Task<DeskDto> UpdateDeskAsync(string id, DeskDto deskDto);
    Task<DeskDto?> GetDeskByIdAsync(string id);
    Task DeleteDeskAsync(string id);
    Task<DeskDto> CloneDeskAsync(string userId, string deskId);
}
