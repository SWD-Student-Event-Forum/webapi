﻿using EventZone.Domain.DTOs.EventOrderDTOs;
using EventZone.Domain.Enums;
using EventZone.Repositories.Commons;
using EventZone.Repositories.Extensions;
using EventZone.Repositories.Helper;
using EventZone.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventZone.WebAPI.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class EventOrderController : Controller
    {
        private readonly IEventOrderService _eventOrderService;
        private readonly INotificationService _notificationService;

        public EventOrderController(IEventOrderService eventOrderService, INotificationService notificationService)
        {
            _eventOrderService = eventOrderService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get list order by eventId
        /// </summary>
        /// <response code="200">Returns a list of order</response>
        /// <response code="400">Event Id is not exist</response>
        /// <response code="404">Not Found</response>
        [HttpGet("event/{id}/event-orders")]
        public async Task<ActionResult<List<EventOrderReponseDTO>>> GetEventOrders(Guid id, [FromQuery] OrderParams orderParam)
        {
            try
            {
                var orders = await _eventOrderService.GetEventOrders(id, orderParam);
                Response.AddPaginationHeader(orders.MetaData);

                var result = new
                {
                    success = true,
                    data = orders,
                    orders.MetaData.CurrentPage,
                    orders.MetaData.PageSize,
                    orders.MetaData.TotalCount,
                    orders.MetaData.TotalPages,
                    message = "Get List Of Event Order Successfully!"
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Get a order by orderId
        /// </summary>
        /// <response code="200">Returns a order</response>
        /// /// <response code="400">Event Order Id is not exist</response>
        /// <response code="404">Not Found</response>
        [HttpGet("event-orders/{id}")]
        public async Task<ActionResult<EventOrderReponseDTO>> GetEventOrder(Guid id)
        {
            try
            {
                var result = await _eventOrderService.GetEventOrder(id);
                return Ok(ApiResult<EventOrderReponseDTO>.Succeed(result, "Get A Order" + id + " Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Create a order
        /// </summary>
        /// <response code="200">Returns a order</response>
        [HttpPost("event-orders")]
        public async Task<ActionResult<EventOrderReponseDTO>> CreateEventOrder(CreateEventOrderReponseDTO order)
        {
            try
            {
                var result = await _eventOrderService.CreateEventOrder(order);
                return Ok(ApiResult<EventOrderReponseDTO>.Succeed(result, "Create order successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Update status a order
        /// </summary>
        /// <response code="200">Returns a order</response>
        [HttpPut("event-orders/{id}")]
        public async Task<ActionResult<EventOrderReponseDTO>> UpdateOrderStatus(Guid id, [FromForm] EventOrderStatusEnums eventOrderStatusEnums)
        {
            try
            {
                var result = await _eventOrderService.UpdateOrderStatus(id, eventOrderStatusEnums);
                return Ok(ApiResult<EventOrderReponseDTO>.Succeed(result, "Update order successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// check in order detail product
        /// </summary>
        /// <response code="200">Returns a order</response>
        [HttpPut("event-orders/order-details/{id}")]
        public async Task<ActionResult<EventOrderDetailDTO>> UpdateOrderDetailsStatus(Guid id)
        {
            try
            {
                var result = await _eventOrderService.CheckinProductStuatus(id);
                return Ok(ApiResult<EventOrderDetailDTO>.Succeed(result, "Update order successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }
    }
}