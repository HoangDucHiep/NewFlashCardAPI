using FlashcardApi.Application.ApplicationUser.Dtos;

namespace FlashcardApi.Application.ApplicationUser;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task LogoutAsync(string token);
    Task RegisterAsync(string username, string email, string password);
    Task RequestPasswordResetAsync(string email);
    Task ResetPasswordAsync(string token, string newPassword);
}
