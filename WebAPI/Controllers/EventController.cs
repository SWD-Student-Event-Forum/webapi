﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Commons;
using Repositories.Extensions;
using Repositories.Helper;
using Services.DTO.EventDTOs;
using Services.Interface;

namespace WebAPI.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public EventController(IEventService eventService, IImageService imageService, IMapper mapper)
        {
            _eventService = eventService;
            _imageService = imageService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list events
        /// </summary>
        /// <returns>A list of events</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /events
        ///
        /// </remarks>
        /// <response code="200">Returns list of events</response>
        /// <response code="400">If the item is null</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEventsAsync([FromQuery] EventParams eventParams)
        {
            try
            {
                var events = await _eventService.GetEvent(eventParams);

                Response.AddPaginationHeader(events.MetaData);

                var eventReponseDTOs = new List<EventResponseDTO>();

                foreach (var erdto in events)
                {
                    var eventReponseDTO = _mapper.Map<EventResponseDTO>(erdto);
                    eventReponseDTOs.Add(eventReponseDTO);
                }

                var result = new
                {
                    success = true,
                    data = eventReponseDTOs,
                    CurrentPage = events.MetaData.CurrentPage,
                    PageSize = events.MetaData.PageSize,
                    TotalCount = events.MetaData.TotalCount,
                    TotalPages = events.MetaData.TotalPages,
                    message = "Get List Of Event Successfully!"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Get event by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Event With Id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /events/1
        ///     {
        ///        "id": 1,
        ///        "name": "Item #1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks> 
        /// <response code="200">Returns a event</response>
        /// <response code="404">Event Not Found</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventByIdAsync(int id)
        {
            try
            {
                var eventModel = await _eventService.GetEventById(id);
                return Ok(ApiResult<EventResponseDTO>.Succeed(eventModel, "Get Event Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Create a event
        /// </summary>
        /// <returns>A New Event</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /events
        ///     {
        ///         "name": "Charity Fundraiser for Children's Education",
        ///         "description": "A charity event to raise funds for underprivileged children's education and school supplies.",
        ///         "thumbnailUrl": "https://eventzoneblob.blob.core.windows.net/images/event-thumbnails/419976033_907448214020009_8314703696454832411_n.png",
        ///         "donationStartDate": "2024-05-15T09:00:00.000Z",
        ///         "donationEndDate": "2024-05-30T18:00:00.000Z",
        ///         "eventStartDate": "2024-05-25T10:00:00.000Z",
        ///         "eventEndDate": "2024-05-25T18:00:00.000Z",
        ///         "location": "Central Park, New York City",
        ///         "note": "Please approve this event",
        ///         "userId": 1,
        ///         "eventCategoryId": 1,
        ///         "university": "New York University",
        ///         "status": "PENDING",
        ///         "origanizationStatus": "PREPARING",
        ///         "isDonation": true,
        ///         "totalCost": 25000
        ///      }
        ///
        ///  Note:
        /// 
        ///     origanizationStatus: 
        ///         PREPARING,
        ///         ACCOMPLISHED,
        ///         DELAYED
        ///         CANCELED
        ///         
        ///     status:
        ///         PENDING,
        ///         REJECTED,
        ///         ISFEEDBACK,
        ///         APRROVED,
        ///         DONATING,
        ///         SUCCESSFUL,
        ///         FAILED
        /// </remarks> 
        /// <response code="200">Returns a event</response>
        /// <response code="400">Requied field is null</response>
        [HttpPost]
        public async Task<IActionResult> CreateEventAsync([FromForm] EventCreateDTO createEventModel)
        {
            try
            {
                string imageUrl = null;
                if (createEventModel.ThumbnailUrl != null)
                {
                    imageUrl = await _imageService.UploadImageAsync(createEventModel.ThumbnailUrl, "event-thumbnails");
                }

                var format = new EventDTO
                {
                    Name = createEventModel.Name,
                    Description = createEventModel.Description,
                    ThumbnailUrl = imageUrl,
                    DonationStartDate = createEventModel.DonationStartDate,
                    DonationEndDate = createEventModel.DonationEndDate,
                    EventStartDate = createEventModel.EventStartDate,
                    EventEndDate = createEventModel.EventEndDate,
                    Note = createEventModel.Note,
                    Location = createEventModel.Location,
                    UserId = createEventModel.UserId,
                    EventCategoryId = createEventModel.EventCategoryId,
                    University = createEventModel.University,
                    Status = createEventModel.Status,
                    TotalCost = createEventModel?.TotalCost
                };
                var eventModel = await _eventService.CreateEvent(format);
                return Ok(ApiResult<EventResponseDTO>.Succeed(eventModel, "Create Event Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Update an existing event
        /// </summary>
        /// <param name="id">The ID of the event to update</param>
        /// <param name="updateEventModel">The updated event data</param>
        /// <returns>The updated event</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /events/1
        ///     {
        ///         "name": "Charity Fundraiser for Children's Education Updated",
        ///         "description": "A charity event to raise funds for underprivileged children's education and school supplies Updated.",
        ///         "thumbnailUrl": "https://eventzoneblob.blob.core.windows.net/images/event-thumbnails/419976033_907448214020009_8314703696454832411_n.png",
        ///         "donationStartDate": "2024-05-15T09:00:00.000Z",
        ///         "donationEndDate": "2024-05-30T18:00:00.000Z",
        ///         "eventStartDate": "2024-05-25T10:00:00.000Z",
        ///         "eventEndDate": "2024-05-25T18:00:00.000Z",
        ///         "location": "Central Park, New York City",
        ///         "userId": 1,
        ///         "eventCategoryId": 1,
        ///         "university": "New York University",
        ///         "status": "PENDING",
        ///         "origanizationStatus": "PREPARING",
        ///         "isDonation": true,
        ///         "totalCost": 25000
        ///      }
        ///
        ///  Note:
        /// 
        ///     origanizationStatus: 
        ///         PREPARING,
        ///         ACCOMPLISHED,
        ///         DELAYED
        ///         CANCELED
        ///         
        ///     status:
        ///         PENDING,
        ///         REJECTED,
        ///         ISFEEDBACK,
        ///         APRROVED,
        ///         DONATING,
        ///         SUCCESSFUL,
        ///         FAILED
        /// </remarks> 
        /// <response code="200">Returns the updated event</response>
        /// <response code="400">If the event or the update data is invalid</response>
        /// <response code="404">If the event is not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventAsync(int id, [FromForm] EventCreateDTO updateEventModel)
        {
            try
            {
                string imageUrl = null;
                if (updateEventModel.ThumbnailUrl != null)
                {
                    imageUrl = await _imageService.UploadImageAsync(updateEventModel.ThumbnailUrl, "event-thumbnails");
                }

                var format = new EventDTO
                {
                    Name = updateEventModel.Name,
                    Description = updateEventModel.Description,
                    ThumbnailUrl = imageUrl,
                    DonationStartDate = updateEventModel.DonationStartDate,
                    DonationEndDate = updateEventModel.DonationEndDate,
                    EventStartDate = updateEventModel.EventStartDate,
                    EventEndDate = updateEventModel.EventEndDate,
                    Note = updateEventModel.Note,
                    Location = updateEventModel.Location,
                    UserId = updateEventModel.UserId,
                    EventCategoryId = updateEventModel.EventCategoryId,
                    University = updateEventModel.University,
                    Status = updateEventModel.Status,
                    TotalCost = updateEventModel.TotalCost ?? 0
                };
                var updatedEvent = await _eventService.UpdateEvent(id, format);
                return Ok(ApiResult<EventResponseDTO>.Succeed(updatedEvent, "Event updated successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Delete an existing event
        /// </summary>
        /// <param name="id">The ID of the event to delete</param>
        /// <returns>The deleted event</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /events/1
        ///
        /// </remarks>
        /// <response code="200">Returns the deleted event</response>
        /// <response code="400">If the event is invalid</response>
        /// <response code="404">If the event is not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventAsync(int id)
        {
            try
            {
                var deletedEvent = await _eventService.DeleteEvent(id);
                return Ok(ApiResult<EventResponseDTO>.Succeed(deletedEvent, "Event deleted successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }
    }
}
