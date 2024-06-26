﻿using Repositories.Commons;
using Repositories.Models;
using Repositories.Models.PackageModels;
using Services.DTO.EventPackageModels;
using Services.DTO.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IEventPackageService
    {
        Task<ResponseGenericModel<EventPackageDetailDTO>> CreatePackageWithProducts(int eventId, string thumbnailurl, CreatePackageRequest newPackage);

        Task<List<EventPackageDetailDTO>> GetAllWithProducts();

        Task<List<EventPackageDetailDTO>> GetAllPackageOfEvent(int eventId);

        Task<List<ProductInPackageDTO>> GetProductsInPackagesWithProduct_Package();

        Task<ResponseGenericModel<List<EventPackageDetailDTO>>> DeleteEventPackagesAsync(List<int> packageIds);

        Task<Pagination<EventPackageDetailDTO>> GetPackagessByFiltersAsync(PaginationParameter paginationParameter, PackageFilterModel packageFilterModel);
    }
}