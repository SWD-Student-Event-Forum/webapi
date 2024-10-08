﻿using EventZone.Domain.DTOs.EventDonationDTOs;
using EventZone.Repositories.Commons;
using EventZone.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventZone.WebAPI.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class EventDonationController : ControllerBase
    {
        private readonly IEventDonationService _eventDonationService;

        public EventDonationController(IEventDonationService eventCampaignService)
        {
            _eventDonationService = eventCampaignService;
        }

        [HttpPost("event-donations")]
        public async Task<IActionResult> Donation(EventDonationCreateDTO model)
        {
            try
            {
                var result = await _eventDonationService.AddDonationToCampaign(model);
                if (result.IsSuccess == false)
                {
                    return BadRequest(ApiResult<object>.Error(null, "Added failed"));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        [HttpGet("campaigns/{id}/event-donations")]
        public async Task<IActionResult> GetCampaignAsync(Guid id)
        {
            try
            {
                var result = await _eventDonationService.GetAllDonationOfCampaign(id);
                return Ok(ApiResult<List<EventDonationDetailDTO>>.Succeed(result, "Get A Order" + id + " Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        [HttpGet("event-donations/me")]
        public async Task<IActionResult> GetMyDonation()
        {
            try
            {
                var result = await _eventDonationService.GetMyDonation();
                return Ok(ApiResult<List<EventDonationDetailDTO>>.Succeed(result, "Get My Donation Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }
    }
}