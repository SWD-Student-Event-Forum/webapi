﻿namespace EventZone.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IEventRepository EventRepository { get; }
        IEventCategoryRepository EventCategoryRepository { get; }
        IEventProductRepository EventProductRepository { get; }
        IEventPackageRepository EventPackageRepository { get; }
        IWalletRepository WalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IEventFeedbackRepository EventFeedbackRepository { get; }
        IEventOrderRepository EventOrderRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IEventCampaignRepository EventCampaignRepository { get; }
        IEventDonationRepository EventDonationRepository { get; }
        IEventTicketRepository EventTicketRepository { get; }
        IEventBoardRepository EventBoardRepository { get; }
        IEventBoardLabelRepository EventBoardLabelRepository { get; }
        IEventBoardTaskLabelRepository EventBoardTaskLabelRepository { get; }
        IEventBoardColumnRepository EventBoardColumnRepository { get; }
        IAttendeeRepository AttendeeRepository { get; }
        IEventBoardTaskRepository EventBoardTaskRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        IEventStaffRepository EventStaffRepository { get; }

        Task<int> SaveChangeAsync();
    }
}