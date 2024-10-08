﻿using EventZone.Domain.DTOs.EventPackageDTOs;
using EventZone.Repositories.Commons;
using EventZone.Repositories.Models.PackageModels;
using EventZone.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventZone.WebAPI.Controllers
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

        /// <summary>
        /// Create an package with event product included
        /// </summary>
        /// <returns>the added event package</returns>
        ///    /// <remarks>
        /// Sample request:
        ///
        ///     POST /event-packages
        ///     {
        ///         "eventId":1,
        ///         "description": "Nice package for student with free purchase",
        ///         "products":[{"productid": 1,"quantity": 10}, {"productid": 1,"quantity": 10}],
        ///         "thumbnailUrl": "any input"
        ///      }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("event-packages")]
        public async Task<IActionResult> CreateAsync([FromQuery] Guid eventId, [FromForm] CreatePackageRequest package)
        {
            try
            {
                string url = "";
                if (package.Thumbnail != null)
                {
                    url = await _imageService.UploadImageAsync(package.Thumbnail, "package-thumbnails");
                }
                var result = await _eventPackageService.CreatePackageWithProducts(eventId, url, package);

                if (result.IsSuccess)
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

        /// <summary>
        /// Get list packages with products included
        /// </summary>
        /// <returns>A list of packages</returns>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("event-packages")]
        public async Task<IActionResult> GetPackagesByFilters([FromQuery] PaginationParameter paginationParameter, [FromQuery] PackageFilterModel packageFilterModel)
        {
            try
            {
                var result = await _eventPackageService.GetPackagessByFiltersAsync(paginationParameter, packageFilterModel);
                if (result == null)
                {
                    return NotFound("No accounts found with the specified filters.");
                }
                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(ApiResult<Pagination<EventPackageDetailDTO>>.Succeed(result, "Get list products successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get list event packages of an event
        /// </summary>
        /// <returns>A list of event packages</returns>
        [HttpGet("events/{eventid}/event-packages")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllPackagesInEventAsync(Guid eventid)
        {
            try
            {
                var data = await _eventPackageService.GetAllPackageOfEvent(eventid);
                return Ok(ApiResult<List<EventPackageDetailDTO>>.Succeed(data, "Get all package of this event: " + eventid));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove a list of event packages by their id
        /// </summary>
        /// <returns>A list of event packages removed</returns>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("event-packages/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _eventPackageService.DeleteEventProductByIdAsync(id);
                if (result.IsSuccess)
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

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <response code="200">Returns a product</response>
        [HttpGet("event-packages/{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            try
            {
                var result = await _eventPackageService.GetPackageById(id);
                if (result == null)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}