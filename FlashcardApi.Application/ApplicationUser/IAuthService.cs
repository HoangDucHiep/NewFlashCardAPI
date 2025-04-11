using FlashcardApi.Application.ApplicationUser.Dtos;

namespace FlashcardApi.Application.ApplicationUser;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task RegisterAsync(string username, string password);
    Task LogoutAsync(string token);
}
