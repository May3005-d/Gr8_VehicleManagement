using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IPromotionService
    {
        // CRUD Operations
        Task<ServiceResponse<List<PromotionDto>>> GetAllPromotionsAsync(
            string? searchTerm = null,
            bool? isActive = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10);
        
        Task<ServiceResponse<PromotionDto>> GetPromotionByIdAsync(Guid id);
        Task<ServiceResponse<PromotionDto>> GetPromotionByCodeAsync(string code);
        Task<ServiceResponse<PromotionDto>> CreatePromotionAsync(CreatePromotionDto dto);
        Task<ServiceResponse<PromotionDto>> UpdatePromotionAsync(UpdatePromotionDto dto);
        Task<ServiceResponse<bool>> DeletePromotionAsync(Guid id);
        
        // Promotion Application
        Task<ServiceResponse<PromotionApplicationResult>> ApplyPromotionAsync(ApplyPromotionDto dto);
        Task<ServiceResponse<List<PromotionDto>>> GetApplicablePromotionsAsync(
            decimal orderValue,
            List<Guid> vehicleIds,
            Guid? customerId = null);
        
        // Validation
        Task<ServiceResponse<bool>> ValidatePromotionAsync(string code, decimal orderValue, List<Guid> vehicleIds, Guid? customerId);
        
        // Statistics
        Task<ServiceResponse<int>> GetTotalPromotionsCountAsync();
        Task<ServiceResponse<int>> GetActivePromotionsCountAsync();
        Task<ServiceResponse<decimal>> GetTotalDiscountGivenAsync(DateTime? startDate = null, DateTime? endDate = null);
        
        // Usage Tracking
        Task<ServiceResponse<bool>> IncrementPromotionUsageAsync(Guid promotionId);
        Task<ServiceResponse<List<PromotionDto>>> GetMostUsedPromotionsAsync(int count = 10);
    }
}
