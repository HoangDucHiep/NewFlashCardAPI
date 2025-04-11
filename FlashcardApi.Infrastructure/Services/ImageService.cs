using FlashcardApi.Application.Image;
using FlashcardApi.Application.Image.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FlashcardApi.Infrastructure.Services;

public class ImageService : IImageService
{
    private readonly IImageRepository _imageRepository;
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads"); // Thư mục lưu ảnh

    public ImageService(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);
    }

    public async Task<ImageDto> UploadImageAsync(string userId, IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var image = new Image
        {
            Url = $"/Uploads/{fileName}", // URL tương đối
            UploadedBy = userId,
        };
        await _imageRepository.AddAsync(image);

        return new ImageDto
        {
            Id = image.Id,
            Url = image.Url,
            UploadedAt = image.UploadedAt,
        };
    }

    public async Task DeleteImageAsync(string id)
    {
        var image = await _imageRepository.AddAsync(new Image { Id = id }); // Giả lập để lấy thông tin (thực tế cần GetById nếu có)
        if (image == null)
            throw new Exception("Image not found");

        var filePath = Path.Combine(_storagePath, Path.GetFileName(image.Url));
        if (File.Exists(filePath))
            File.Delete(filePath);

        await _imageRepository.DeleteAsync(id);
    }
}
