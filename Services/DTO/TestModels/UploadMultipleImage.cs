﻿using Microsoft.AspNetCore.Http;

namespace Services.DTO.TestModels
{
    public class UploadMultipleImage
    {
        public List<IFormFile> fileImages { get; set; }
        public string OthersField { get; set; }
    }
}
