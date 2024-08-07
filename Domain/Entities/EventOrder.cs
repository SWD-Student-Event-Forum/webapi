﻿namespace Domain.Entities
{
    public class EventOrder : BaseEntity
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public Int64 TotalAmount { get; set; }
        public string Status { get; set; }

        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<EventOrderDetail> EventOrderDetails { get; set; }
        public virtual ICollection<TransactionDetail> TransactionDetail { get; set; }
    }
}
