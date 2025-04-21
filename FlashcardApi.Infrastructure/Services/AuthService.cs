using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
            throw new Exception("Email does not exist");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new Exception("Incorrect password");


        // Thu hồi token cũ nếu tồn tại
        if (!string.IsNullOrEmpty(user.CurrentToken))
        {
            await _revokedTokenRepository.AddAsync(
                new RevokedToken
                {
                    Token = user.CurrentToken,
                    ExpiresAt = DateTime.UtcNow.AddYears(1),
                }
            );
        }

        // Tạo token mới
        var token = GenerateJwtToken(user);

        // Lưu token mới vào bảng người dùng
        user.CurrentToken = token;
        await _userManager.UpdateAsync(user);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.UserName,
            Email = user.Email,
        };
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

    public async Task RegisterAsync(string username, string email, string password)
    {
        // Kiểm tra định dạng email
        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(email))
            throw new Exception("Invalid email format");

        // Kiểm tra mật khẩu
        if (!IsValidPassword(password))
            throw new Exception("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

        // Kiểm tra email đã tồn tại
        var existingEmailUser = await _userManager.FindByEmailAsync(email);
        if (existingEmailUser != null)
            throw new Exception("Email is already registered");

        // Kiểm tra username đã tồn tại
        var existingUsernameUser = await _userManager.FindByNameAsync(username);
        if (existingUsernameUser != null)
            throw new Exception("Username is already taken");

        var user = new ApplicationUser { UserName = username, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception(
                "Registration failed: "
                    + string.Join(", ", result.Errors.Select(e => e.Description))
            );
    }

    private bool IsValidPassword(string password)
    {
        // Kiểm tra mật khẩu: ít nhất 1 chữ hoa, 1 chữ thường, 1 số, 1 ký tự đặc biệt
        var regex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return regex.IsMatch(password);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
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
