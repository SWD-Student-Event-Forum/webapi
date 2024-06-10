﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.EventPackageModels;
using Services.Interface;

namespace WebAPI.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class EventPackageController : ControllerBase
    {
        private readonly IEventPackageService _eventPackageService;
        private readonly IImageService _imageService;

        public EventPackageController(IEventPackageService eventPackageService, IImageService imageService)
        {
            _eventPackageService = eventPackageService;
            _imageService = imageService;
        }

        [HttpPost("event-packages")]
        public async Task<IActionResult> CreateAsync([FromQuery] int eventId, [FromForm] CreatePackageRequest package)
        {
            try
            {
                string url = "";
                if (package.Thumbnail != null)
                {
                    url = await _imageService.UploadImageAsync(package.Thumbnail, "package-thumbnails");
                }
                var result = await _eventPackageService.CreatePackageWithProducts(eventId, url, package);

                if (result.Status)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("products/event-packages")]
        public async Task<IActionResult> GetProductsInPackagesWithProduct_Package()
        {
            try
            {
                var data = await _eventPackageService.GetProductsInPackagesWithProduct_Package();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("event-packages")]
        public async Task<IActionResult> GetAllPackageAsync()
        {
            try
            {
                var data = await _eventPackageService.GetAllWithProducts();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{eventid}/event-packages")]
        public async Task<IActionResult> GetAllPackagesInEventAsync(int eventid)
        {
            try
            {
                var data = await _eventPackageService.GetAllPackageOfEvent(eventid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromBody] List<int> packageIds)
        {
            try
            {
                var result = await _eventPackageService.DeleteEventPackagesAsync(packageIds);
                if (result.Status)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}