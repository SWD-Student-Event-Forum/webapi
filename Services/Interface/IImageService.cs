﻿using EventZone.Domain.DTOs.ImageDTOs;
using Microsoft.AspNetCore.Http;

namespace EventZone.Services.Interface
{
    public interface IImageService
    {
        public Task<string> UploadImageAsync(IFormFile file, string folderName);

        public Task<List<ImageReturnDTO>> UploadMultipleImagesAsync(List<IFormFile> fileImages, string folderName);

        public Task<bool> DeleteImageAsync(string publicId);

        Task<List<ImageReturnDTO>> UploadImageRangeAsync(List<IFormFile> fileImages, string folderName);
    }
}