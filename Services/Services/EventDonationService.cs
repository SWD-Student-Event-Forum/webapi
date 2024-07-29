﻿using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Repositories.Commons;
using Repositories.Interfaces;
using Services.DTO.EventDonationDTOs;
using Services.DTO.ResponseModels;
using Services.Interface;

namespace Services.Services
{
    public class EventDonationService : IEventDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly INotificationService _notificationService;
        private readonly IWalletService _walletService;

        public EventDonationService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, INotificationService notificationService, IWalletService walletService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            _notificationService = notificationService;
            _walletService = walletService;
        }

        public async Task<ApiResult<EventDonationDetailDTO>> AddDonationToCampaign(EventDonationCreateDTO data)
        {
            try
            {
                var checkEvent = await _unitOfWork.EventCampaignRepository.GetByIdAsync(data.EventCampaignId, x => x.Event);
                var checkUser = _claimsService.GetCurrentUserId == -1 ? 1 : _claimsService.GetCurrentUserId; //test

                if (checkEvent == null)
                {
                    return new ApiResult<EventDonationDetailDTO>()
                    {
                        Success = false,
                        Message = " This campaign is not existed",
                        Data = null
                    };
                }

                var newDonation = new EventDonation
                {
                    EventCampaignId = data.EventCampaignId,
                    UserId = checkUser,
                    Amount = data.Amount,
                    DonationDate = DateTime.UtcNow.AddHours(7)
                };

                checkEvent.CollectedAmount += newDonation.Amount;

                if (checkEvent.CollectedAmount > checkEvent.GoalAmount)
                {
                    return new ApiResult<EventDonationDetailDTO>()
                    {
                        Success = false,
                        Message = "added failed, cannot donate more than goal amount",
                        Data = _mapper.Map<EventDonationDetailDTO>(newDonation)
                    };
                }
                await _unitOfWork.EventCampaignRepository.Update(checkEvent);

                var newData = await _unitOfWork.EventDonationRepository.AddAsync(newDonation);
                var result = await _unitOfWork.SaveChangeAsync();
                if (result > 0)
                {
                    // Send notification
                    var notification = new Notification
                    {
                        Title = "Donation Successfully!",
                        Body = "Amount: " + newDonation.Amount,
                        UserId = newDonation.UserId,
                        Url = "/profile",
                        Sender = "System"
                    };

                    await _notificationService.PushNotification(notification);

                    // Decrease money in wallet
                    var transation = await _unitOfWork.WalletRepository.Donation(newDonation.UserId, newDonation.Amount);

                    // Increase money of event owner
                    var eventOwnerWallet = await _unitOfWork.WalletRepository.GetWalletByUserIdAndType(checkEvent.EventId, WalletTypeEnums.PERSONAL);
                    eventOwnerWallet.Balance += newDonation.Amount;
                    await _unitOfWork.WalletRepository.Update(eventOwnerWallet);
                    await _unitOfWork.SaveChangeAsync();

                    //Notification to event order
                    var notificationToOrganizer = new Notification
                    {
                        Title = "One person donate event ",
                        Body = "Amount: " + newDonation.Amount,
                        UserId = checkEvent.Event.UserId,
                        Url = "/dashboard/my-events/" + checkEvent.EventId,
                        Sender = "System"
                    };
                    await _notificationService.PushNotification(notificationToOrganizer);

                    return new ApiResult<EventDonationDetailDTO>()
                    {
                        Success = true,
                        Message = "Successfully added",
                        Data = _mapper.Map<EventDonationDetailDTO>(newData)
                    };
                }
                else
                {
                    throw new Exception("Added failed please check");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EventDonationDetailDTO>> GetAllDonationOfCampaign(int id)
        {
            var result = await _unitOfWork.EventDonationRepository.GetAllDonationByCampaignId(id);
            return _mapper.Map<List<EventDonationDetailDTO>>(result);
        }

        public async Task<List<EventDonationDetailDTO>> GetMyDonation()
        {
            var userId = _claimsService.GetCurrentUserId;
            if (userId == -1) throw new Exception("you are not login or bearer is not correct");
            var result = await _unitOfWork.EventDonationRepository.GetMyDonation();
            return _mapper.Map<List<EventDonationDetailDTO>>(result);
        }
    }
}