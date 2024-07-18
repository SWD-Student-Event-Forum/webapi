﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class EventPackageDetailDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string? Title { get; set; }

        public Int64 TotalPrice { get; set; }
        public string Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public virtual ICollection<ProductInPackageDTO>? ProductsInPackage { get; set; }

        //tam thoi
        public List<EventProductDetailDTO>? Products { get; set; }
    }
}