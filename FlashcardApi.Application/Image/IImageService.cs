using FlashcardApi.Application.Image.Dtos;
using Microsoft.AspNetCore.Http;

namespace FlashcardApi.Application.Image;

public interface IImageService
{
    Task<ImageDto> UploadImageAsync(string userId, IFormFile file);
    Task DeleteImageAsync(string id);
}
