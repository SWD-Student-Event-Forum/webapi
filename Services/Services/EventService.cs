﻿using AutoMapper;
using Domain.Entities;
using Repositories.Helper;
using Repositories.Interfaces;
using Services.DTO.EventDTOs;
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

        public async Task<PagedList<Event>> GetEvent(EventParams eventParams)
        {
            var query = _unitOfWork.EventRepository.FilterAllField(eventParams).AsQueryable();

            var events = await PagedList<Event>.ToPagedList(query, eventParams.PageNumber, eventParams.PageSize);

            //.Sort(eventParams.OrderBy);

            return events;
        }

        public async Task<EventResponseDTO> GetEventById(int id)
        {
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id, x => x.EventCategory);

            if (existingEvent == null)
            {
                throw new Exception("Event not found");
            }

            var result = _mapper.Map<EventResponseDTO>(existingEvent);
            return result;
        }
        public async Task<EventResponseDTO> CreateEvent(EventDTO eventModel)
        {
            var eventEntity = _mapper.Map<Event>(eventModel);
            //check user
            var user = await _unitOfWork.UserRepository.GetAllUsersAsync();
            var isExistUser = user.FirstOrDefault(x => x.Id == eventModel.UserId);
            if (isExistUser == null)
            {
                throw new Exception("User not null when create event");
            }
            eventEntity.User = isExistUser;
            //check eventCategory
            var eventCategory = await _unitOfWork.EventCategoryRepository.GetByIdAsync(eventModel.EventCategoryId);
            if (eventCategory == null)
            {
                throw new Exception("Event category not null when create event");
            }
            eventEntity.EventCategory = eventCategory;

            var newEvent = await _unitOfWork.EventRepository.AddAsync(eventEntity);

            var result = _mapper.Map<EventResponseDTO>(newEvent);
            await _unitOfWork.SaveChangeAsync();
            return result;
        }

        public async Task<EventResponseDTO> UpdateEvent(int id, EventDTO eventModel)
        {
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id);

            if (existingEvent == null)
            {
                throw new Exception("Event not found");
            }
            //check user
            var user = await _unitOfWork.UserRepository.GetAllUsersAsync();
            var isExistUser = user.FirstOrDefault(x => x.Id == eventModel.UserId);
            if (isExistUser == null)
            {
                throw new Exception("User does not exist!");
            }
            existingEvent.User = isExistUser;
            //check eventCategory
            var eventCategory = await _unitOfWork.EventCategoryRepository.GetByIdAsync(eventModel.EventCategoryId);
            if (eventCategory == null)
            {
                throw new Exception("Event category does not exist!");
            }
            existingEvent.EventCategory = eventCategory;

            existingEvent.Name = eventModel.Name ?? existingEvent.Name;
            existingEvent.Description = eventModel.Description ?? existingEvent.Description;
            existingEvent.ThumbnailUrl = eventModel.ThumbnailUrl ?? existingEvent.ThumbnailUrl;
            existingEvent.DonationStartDate = eventModel.DonationStartDate ?? existingEvent.DonationStartDate;
            existingEvent.DonationEndDate = eventModel.DonationEndDate ?? existingEvent.DonationEndDate;
            existingEvent.EventStartDate = eventModel.EventStartDate ?? existingEvent.EventStartDate;
            existingEvent.EventEndDate = eventModel.EventEndDate ?? existingEvent.EventEndDate;
            existingEvent.Location = eventModel.Location ?? existingEvent.Location;
            existingEvent.University = eventModel.University ?? existingEvent.University;
            existingEvent.Status = eventModel.Status.ToString() ?? existingEvent.Status;
            existingEvent.OriganizationStatus = eventModel.OriganizationStatus.ToString() ?? existingEvent.OriganizationStatus;
            existingEvent.IsDonation = eventModel.IsDonation ?? existingEvent.IsDonation;
            existingEvent.TotalCost = eventModel.TotalCost ?? existingEvent.TotalCost;

            var isUpdated = await _unitOfWork.EventRepository.Update(existingEvent);

            if (isUpdated == false)
            {
                throw new Exception("Failed to update event");
            }

            var result = _mapper.Map<EventResponseDTO>(existingEvent);
            await _unitOfWork.SaveChangeAsync();
            return result;
        }
        public async Task<EventResponseDTO> DeleteEvent(int id)
        {
            var existingEvent = await _unitOfWork.EventRepository.GetByIdAsync(id);

            if (existingEvent == null)
            {
                throw new Exception("Event not found");
            }

            var isDeleted = await _unitOfWork.EventRepository.SoftRemove(existingEvent);
            if (isDeleted)
            {
                var result = _mapper.Map<EventResponseDTO>(existingEvent);
                await _unitOfWork.SaveChangeAsync();
                return result;
            }
            else
            {
                throw new Exception("Failed to delete event");
            }
        }
    }
}
