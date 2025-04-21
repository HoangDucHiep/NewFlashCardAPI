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

    public async Task<ImageDto> UploadImageAsync(string userId, IFormFile file)
    {
        var filePath = Path.Combine(_storagePath, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var image = new Image { Url = $"/Uploads/{file.FileName}", UploadedBy = userId };
        var createdImage = await _imageRepository.AddAsync(image);

        return new ImageDto
        {
            Id = createdImage.Id,
            FileName = file.FileName,
            Url = createdImage.Url,
            UploadedAt = createdImage.UploadedAt,
        };
    }

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        var image = await _imageRepository.GetByFileNameAsync(fileName);
        if (image == null)
            return false;

        var filePath = Path.Combine(_storagePath, fileName);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Không thể xóa file: {ex.Message}");
                return false; // Hoặc tùy bạn xử lý
            }
        }

        return await _imageRepository.DeleteAsync(image.Id);
    }
}
