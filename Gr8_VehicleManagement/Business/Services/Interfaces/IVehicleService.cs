using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IVehicleService
    {
        // Vehicle Models
        Task<ServiceResponse<List<VehicleModelDto>>> GetAllModelsAsync(
            string? searchTerm = null,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 10);
        
        Task<ServiceResponse<VehicleModelDto>> GetModelByIdAsync(Guid id);
        Task<ServiceResponse<VehicleModelDto>> GetVehicleModelByIdAsync(Guid id);
        Task<ServiceResponse<VehicleModelDto>> CreateModelAsync(VehicleModelDto dto);
        Task<ServiceResponse<VehicleModelDto>> CreateVehicleModelAsync(CreateVehicleModelDto dto);
        Task<ServiceResponse<VehicleModelDto>> UpdateModelAsync(VehicleModelDto dto);
        Task<ServiceResponse<VehicleModelDto>> UpdateVehicleModelAsync(UpdateVehicleModelDto dto);
        Task<ServiceResponse<bool>> DeleteModelAsync(Guid id);
        Task<ServiceResponse<List<string>>> GetCategoriesAsync();
        
        // Vehicle Versions
        Task<ServiceResponse<List<VehicleVersionDto>>> GetVersionsByModelIdAsync(Guid modelId);
        Task<ServiceResponse<VehicleVersionDto>> GetVersionByIdAsync(Guid id);
        Task<ServiceResponse<VehicleVersionDto>> CreateVersionAsync(VehicleVersionDto dto);
        Task<ServiceResponse<VehicleVersionDto>> UpdateVersionAsync(VehicleVersionDto dto);
        Task<ServiceResponse<bool>> DeleteVersionAsync(Guid id);
        
        // Vehicle Inventory
        Task<ServiceResponse<List<VehicleInventoryDto>>> GetInventoryAsync(
            Guid? modelId = null,
            Guid? dealerId = null,
            string? status = null);
        Task<ServiceResponse<int>> GetAvailableStockAsync(Guid versionId, Guid? dealerId = null);
        
        // Statistics
        Task<ServiceResponse<int>> GetTotalModelsCountAsync();
        Task<ServiceResponse<int>> GetTotalInventoryCountAsync();
    }
}

