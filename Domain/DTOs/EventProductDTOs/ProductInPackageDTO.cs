﻿namespace EventZone.Domain.DTOs.EventProductDTOs
{
    public class ProductInPackageDTO
    {
        public int ProductId { get; set; }
        public int PackageId { get; set; }
        public int Quantity { get; set; }

        public virtual EventProductDetailDTO? EventProduct { get; set; }
    }
}