﻿using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO.EventDTOs
{
    public class EventCreateDTO
    {
        [Required]
        [BindProperty(Name = "name")]
        public string Name { get; set; }

        [BindProperty(Name = "description")]
        public string? Description { get; set; }

        [BindProperty(Name = "thumbnail-url")]
        public IFormFile? ThumbnailUrl { get; set; }

        [BindProperty(Name = "donation-start-date")]
        public DateTime? DonationStartDate { get; set; }

        [BindProperty(Name = "donation-end-date")]
        public DateTime? DonationEndDate { get; set; }

        [BindProperty(Name = "event-start-date")]
        public DateTime? EventStartDate { get; set; }

        [BindProperty(Name = "event-end-date")]
        public DateTime? EventEndDate { get; set; }

        [Required]
        [BindProperty(Name = "note")]
        public string Note { get; set; }

        [BindProperty(Name = "location")]
        public string? Location { get; set; }

        [Required]
        [BindProperty(Name = "user-id")]
        public int UserId { get; set; }

        [Required]
        [BindProperty(Name = "event-category-id")]
        public int EventCategoryId { get; set; }

        [BindProperty(Name = "university")]
        public string? University { get; set; }

        [Required]
        [BindProperty(Name = "status")]
        public EventStatusEnums Status { get; set; } = EventStatusEnums.PENDING;

        [BindProperty(Name = "total-cost")]
        public Int64? TotalCost { get; set; }
    }
}
