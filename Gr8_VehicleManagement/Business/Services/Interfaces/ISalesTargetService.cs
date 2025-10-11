using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface ISalesTargetService
    {
        Task<ServiceResponse<List<SalesTargetDto>>> GetAllSalesTargetsAsync(
            string? searchTerm = null,
            TargetFor? targetFor = null,
            Guid? dealerId = null,
            Guid? userId = null,
            bool? isActive = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<ServiceResponse<SalesTargetDto>> GetSalesTargetByIdAsync(Guid id);
        Task<ServiceResponse<SalesTargetDto>> CreateSalesTargetAsync(CreateSalesTargetDto dto);
        Task<ServiceResponse<SalesTargetDto>> UpdateSalesTargetAsync(UpdateSalesTargetDto dto);
        Task<ServiceResponse<bool>> DeleteSalesTargetAsync(Guid id);
        Task<ServiceResponse<List<SalesTargetSummaryDto>>> GetSalesTargetSummaryAsync(
            Guid? dealerId = null,
            Guid? userId = null,
            bool? isActive = null);
        Task<ServiceResponse<decimal>> CalculateCommissionAsync(Guid salesTargetId, decimal achievementAmount);
        Task<ServiceResponse<bool>> UpdateAchievementAsync(Guid salesTargetId, decimal amount, int quantity);
        Task<ServiceResponse<List<SalesTargetDto>>> GetActiveTargetsForUserAsync(Guid userId);
        Task<ServiceResponse<List<SalesTargetDto>>> GetActiveTargetsForDealerAsync(Guid dealerId);
        Task<ServiceResponse<int>> GetTotalSalesTargetsCountAsync();
        Task<ServiceResponse<int>> GetActiveSalesTargetsCountAsync();
        Task<ServiceResponse<decimal>> GetTotalCommissionEarnedAsync(Guid? dealerId = null, Guid? userId = null);
        Task<ServiceResponse<List<SalesTargetDto>>> GetTopPerformersAsync(int count = 10);
    }
}
