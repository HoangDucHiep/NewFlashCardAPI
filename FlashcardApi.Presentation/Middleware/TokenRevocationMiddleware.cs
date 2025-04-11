using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FlashcardApi.Presentation.Middleware
{
    public class TokenRevocationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenRevocationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<
                        UserManager<ApplicationUser>
                    >();
                    var revokedTokenRepository =
                        scope.ServiceProvider.GetRequiredService<IRevokedTokenRepository>();

                    // Kiểm tra token đã bị thu hồi
                    if (await revokedTokenRepository.IsTokenRevokedAsync(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token has been revoked.");
                        return;
                    }

                    // Kiểm tra token có khớp với CurrentToken của người dùng
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var userId = jwtToken
                        .Claims.First(c => c.Type == ClaimTypes.NameIdentifier)
                        .Value;
                    var user = await userManager.FindByIdAsync(userId);

                    if (user == null || user.CurrentToken != token)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid token.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    public static class TokenRevocationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenRevocation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenRevocationMiddleware>();
        }
    }
}
