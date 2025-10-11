using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Feedbacks
{
    public class IndexModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;

        public IndexModel(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public List<FeedbackDto> Feedbacks { get; set; } = new List<FeedbackDto>();
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        
        // Statistics
        public int TotalFeedbacks { get; set; }
        public int OpenFeedbacks { get; set; }
        public int OverdueFeedbacks { get; set; }
        public decimal AverageResolutionTime { get; set; }
        
        // Filter properties
        public string? SearchTerm { get; set; }
        public string? FeedbackType { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public bool? IsOverdue { get; set; }
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public async Task<IActionResult> OnGetAsync(
            string? searchTerm = null,
            string? feedbackType = null,
            string? status = null,
            string? priority = null,
            bool? isOverdue = null,
            int? pageNumber = null)
        {
            // Get current user type for permissions
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff || userType == UserType.Customer;
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff;

            // Set filter properties
            SearchTerm = searchTerm;
            FeedbackType = feedbackType;
            Status = status;
            Priority = priority;
            IsOverdue = isOverdue;
            CurrentPage = pageNumber ?? 1;

            try
            {
                // Parse enums
                FeedbackType? feedbackTypeEnum = null;
                if (!string.IsNullOrEmpty(feedbackType) && Enum.TryParse<FeedbackType>(feedbackType, out var parsedFeedbackType))
                {
                    feedbackTypeEnum = parsedFeedbackType;
                }

                FeedbackStatus? statusEnum = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<FeedbackStatus>(status, out var parsedStatus))
                {
                    statusEnum = parsedStatus;
                }

                Priority? priorityEnum = null;
                if (!string.IsNullOrEmpty(priority) && Enum.TryParse<Priority>(priority, out var parsedPriority))
                {
                    priorityEnum = parsedPriority;
                }

                // Get feedbacks
                var result = await _feedbackService.GetAllFeedbacksAsync(
                    searchTerm: searchTerm,
                    feedbackType: feedbackTypeEnum,
                    status: statusEnum,
                    priority: priorityEnum,
                    isOverdue: isOverdue,
                    pageNumber: CurrentPage,
                    pageSize: PageSize);

                if (result.Success && result.Data != null)
                {
                    Feedbacks = result.Data;
                }

                // Get statistics
                await LoadStatistics();

                // Calculate pagination
                var totalCountResult = await _feedbackService.GetTotalFeedbacksCountAsync();
                if (totalCountResult.Success)
                {
                    TotalPages = (int)Math.Ceiling((double)totalCountResult.Data / PageSize);
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error loading feedbacks: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách phản hồi.";
                return Page();
            }
        }

        private async Task LoadStatistics()
        {
            try
            {
                // Get total feedbacks count
                var totalCountResult = await _feedbackService.GetTotalFeedbacksCountAsync();
                if (totalCountResult.Success)
                {
                    TotalFeedbacks = totalCountResult.Data;
                }

                // Get open feedbacks count
                var openCountResult = await _feedbackService.GetOpenFeedbacksCountAsync();
                if (openCountResult.Success)
                {
                    OpenFeedbacks = openCountResult.Data;
                }

                // Get overdue feedbacks count
                var overdueCountResult = await _feedbackService.GetOverdueFeedbacksCountAsync();
                if (overdueCountResult.Success)
                {
                    OverdueFeedbacks = overdueCountResult.Data;
                }

                // Get average resolution time
                var avgTimeResult = await _feedbackService.GetAverageResolutionTimeAsync();
                if (avgTimeResult.Success)
                {
                    AverageResolutionTime = avgTimeResult.Data;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }
    }
}
