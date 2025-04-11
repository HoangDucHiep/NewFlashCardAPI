using FlashcardApi.Domain.Entities;
using System.Threading.Tasks;

namespace FlashcardApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByUsernameAsync(string username);
        Task<ApplicationUser> FindByIdAsync(string userId);
    }
}