using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<ServiceResponse<List<FeedbackDto>>> GetAllFeedbacksAsync(
            string? searchTerm = null,
            FeedbackType? feedbackType = null,
            FeedbackStatus? status = null,
            Priority? priority = null,
            Guid? assignedUserId = null,
            Guid? customerId = null,
            bool? isOverdue = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<ServiceResponse<FeedbackDto>> GetFeedbackByIdAsync(Guid id);
        Task<ServiceResponse<FeedbackDto>> CreateFeedbackAsync(CreateFeedbackDto dto);
        Task<ServiceResponse<FeedbackDto>> UpdateFeedbackAsync(UpdateFeedbackDto dto);
        Task<ServiceResponse<bool>> DeleteFeedbackAsync(Guid id);
        Task<ServiceResponse<FeedbackDto>> AssignFeedbackAsync(Guid feedbackId, Guid assignedUserId);
        Task<ServiceResponse<FeedbackDto>> ResolveFeedbackAsync(Guid feedbackId, string resolution);
        Task<ServiceResponse<List<FeedbackSummaryDto>>> GetFeedbackSummaryAsync(
            FeedbackStatus? status = null,
            Guid? assignedUserId = null,
            bool? isOverdue = null);
        Task<ServiceResponse<List<FeedbackDto>>> GetMyFeedbacksAsync(Guid userId);
        Task<ServiceResponse<List<FeedbackDto>>> GetOverdueFeedbacksAsync();
        Task<ServiceResponse<int>> GetTotalFeedbacksCountAsync();
        Task<ServiceResponse<int>> GetOpenFeedbacksCountAsync();
        Task<ServiceResponse<int>> GetOverdueFeedbacksCountAsync();
        Task<ServiceResponse<decimal>> GetAverageResolutionTimeAsync();
        Task<ServiceResponse<List<FeedbackDto>>> GetRecentFeedbacksAsync(int count = 10);
    }
}
