using FlashcardApi.Application.Image;
using FlashcardApi.Application.Image.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FlashcardApi.Infrastructure.Services;

public class ImageService : IImageService
{
    private readonly IImageRepository _imageRepository;
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    public ImageService(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);
    }

    public async Task<ImageDto> UploadImageAsync(string userId, IFormFile file, string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var image = new Image
        {
            Url = $"/Uploads/{fileName}",
            UploadedBy = userId,
        };
        await _imageRepository.AddAsync(image);

        return new ImageDto
        {
            Id = image.Id,
            FileName = fileName,
            Url = image.Url,
            UploadedAt = image.UploadedAt,
        };
    }

    public async Task DeleteImageAsync(string fileName)
    {
        var image = await _imageRepository.GetByFileNameAsync(fileName); // Cần thêm phương thức này
        if (image == null)
            throw new Exception("Image not found");

        var filePath = Path.Combine(_storagePath, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);

        await _imageRepository.DeleteAsync(image.Id);
    }
}
