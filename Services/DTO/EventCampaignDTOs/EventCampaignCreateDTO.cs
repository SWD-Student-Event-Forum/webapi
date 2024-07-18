﻿using Domain.Enums;
using Services.DTO.EventDonationDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO.EventCampaignDTOs
{
    public class EventCampaignCreateDTO
    {
        [Required(ErrorMessage = "Please input event for this campaign")]
        public int EventId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventCampaignStatusEnum Status { get; set; }
        public Int64 GoalAmount { get; set; }
    }
}