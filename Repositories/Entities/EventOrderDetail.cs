﻿namespace Repositories.Entities
{
    public class EventOrderDetail : BaseEntity
    {
        public int PackageId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual EventPackage EventPackage { get; set; }
        public virtual EventOrder EventOrder { get; set; }
    }
}