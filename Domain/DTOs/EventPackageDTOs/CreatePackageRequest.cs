﻿using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.EventPackageDTOs
{
    public class CreatePackageRequest
    {
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please input product list to add package")]
        public List<ProductQuantityDTO> Products { get; set; }

        [Required(ErrorMessage = "Please set a title for the package")]
        public string Title { get; set; } = string.Empty;

        public IFormFile? Thumbnail { get; set; }

        private string? _ThumbnailUrl;

        //[Ignore]
        //public string ThumbnailUrl
        //{
        //    get
        //    {
        //        return _ThumbnailUrl;
        //    }
        //    set
        //    {
        //        _ThumbnailUrl =  value;
        //    }
        //}
    }
}