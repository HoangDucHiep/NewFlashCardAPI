using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IImageRepository
{
    Task<Image> AddAsync(Image image);
    Task DeleteAsync(string id);
    Task<Image> GetByFileNameAsync(string fileName);
    Task<List<Image>> GetAllAsync();
}
