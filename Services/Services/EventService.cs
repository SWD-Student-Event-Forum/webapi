﻿using AutoMapper;
using Repositories.DTO;
using Repositories.Interfaces;
using Services.Interface;

namespace Services.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<EventModel> GetEventByIdAsync(int id)
        {
            var Event = await _unitOfWork.EventRepository.GetEventByIdAsync(id);
            var eventFormat = _mapper.Map<EventModel>(Event);
            return eventFormat;
        }

        public async Task<List<EventModel>> GetEventsAsync()
        {
            var Events = await _unitOfWork.EventRepository.GetEventsAsync();
            var result = new List<EventModel>();
            foreach (var Event in Events)
            {
                var eventFormat = _mapper.Map<EventModel>(Event);
                result.Add(eventFormat);
            }

            return result;
        }
    }
}