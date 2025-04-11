using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlashcardApi.Application.ApplicationUser;
using FlashcardApi.Application.ApplicationUser.Dtos;
using FlashcardApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FlashcardApi.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IRevokedTokenRepository _revokedTokenRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IRevokedTokenRepository revokedTokenRepository
    )
    {
        _userManager = userManager;
        _configuration = configuration;
        _revokedTokenRepository = revokedTokenRepository;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new Exception("Invalid credentials");

        // Thu hồi token cũ nếu tồn tại
        if (!string.IsNullOrEmpty(user.CurrentToken))
        {
            await _revokedTokenRepository.AddAsync(
                new RevokedToken
                {
                    Token = user.CurrentToken,
                    ExpiresAt = DateTime.UtcNow.AddYears(1), // Hoặc thời gian hợp lệ
                }
            );
        }

        // Tạo token mới
        var token = GenerateJwtToken(user);

        // Lưu token mới vào bảng người dùng
        user.CurrentToken = token;
        await _userManager.UpdateAsync(user);

        return new LoginResponseDto { Token = token, Username = user.UserName };
    }

    public async Task LogoutAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && user.CurrentToken == token)
        {
            // Thu hồi token
            await _revokedTokenRepository.AddAsync(
                new RevokedToken { Token = token, ExpiresAt = jwtToken.ValidTo }
            );

            // Xóa token khỏi bảng người dùng
            user.CurrentToken = null;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task RegisterAsync(string username, string password)
    {
        var user = new ApplicationUser { UserName = username, Email = username + "@example.com" };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception(
                "Registration failed: "
                    + string.Join(", ", result.Errors.Select(e => e.Description))
            );
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
