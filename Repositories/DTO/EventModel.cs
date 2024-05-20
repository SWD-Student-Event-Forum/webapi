﻿namespace Repositories.DTO
{
    public class EventModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DonationStartDate { get; set; }
        public DateTime? DonationEndDate { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string? Location { get; set; }
        public int UserId { get; set; }
        public string? University { get; set; }
        public string Status { get; set; }
        public string? DonationStatus { get; set; }
        public bool IsDonation { get; set; }
        public decimal TotalCost { get; set; }
        // Base Entity
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
    //public virtual User User { get; set; }
    //public virtual ICollection<EventImage> EventImages { get; set; }
    //public virtual ICollection<EventComment> EventComments { get; set; }
    //public virtual ICollection<Post> Posts { get; set; }
    //public virtual ICollection<EventOrder> EventOrders { get; set; }
    //public virtual ICollection<EventPackage> EventPackages { get; set; }
    //public virtual ICollection<EventProduct> EventProducts { get; set; }
    //public virtual ICollection<EventFeedback> EventFeedbacks { get; set; }
}
