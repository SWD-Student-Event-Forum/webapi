﻿using EventZone.Domain.DTOs.EventBoardDTOs.EventBoardTaskDTOs;

namespace EventZone.Domain.DTOs.EventBoardDTOs.EventBoardColumnDTOs
{
    public class EventBoardColumnDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public List<EventBoardTaskResponseDTO> EventBoardTasks { get; set; }
    }
}
