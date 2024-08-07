﻿using Domain.DTOs.EventCampaignDTOs;
using Domain.DTOs.EventCategoryDTOs;
using Domain.DTOs.EventPackageDTOs;
using Domain.DTOs.UserDTOs;

namespace Domain.DTOs.EventDTOs
{
    public class EventResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int UserId { get; set; }
        public UserDetailsModel User { get; set; }
        public EventCategoryResponseDTO? EventCategory { get; set; }
        public string University { get; set; }
        public string? Status { get; set; }
        public long TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<EventPackageDetailDTO>? EventPackages { get; set; }
        public virtual List<EventCampaignDTO>? EventCampaigns { get; set; }
    }
}