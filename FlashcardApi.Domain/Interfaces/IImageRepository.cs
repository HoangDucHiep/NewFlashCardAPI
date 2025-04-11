using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IImageRepository
{
    Task<Image> AddAsync(Image image);
    Task DeleteAsync(string id);
}
