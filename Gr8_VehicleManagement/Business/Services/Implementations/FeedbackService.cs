using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(ApplicationDbContext context, ILogger<FeedbackService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<FeedbackDto>>> GetAllFeedbacksAsync(
            string? searchTerm = null,
            FeedbackType? feedbackType = null,
            FeedbackStatus? status = null,
            Priority? priority = null,
            Guid? assignedUserId = null,
            Guid? customerId = null,
            bool? isOverdue = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Feedbacks
                    .Include(f => f.Customer)
                    .Include(f => f.AssignedUser)
                    .Include(f => f.Order)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(f => 
                        f.Title.Contains(searchTerm) || 
                        f.Description.Contains(searchTerm) ||
                        (f.Customer != null && f.Customer.FullName.Contains(searchTerm)));
                }

                if (feedbackType.HasValue)
                {
                    query = query.Where(f => f.FeedbackType == feedbackType.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(f => f.Status == status.Value);
                }

                if (priority.HasValue)
                {
                    query = query.Where(f => f.Priority == priority.Value);
                }

                if (assignedUserId.HasValue)
                {
                    query = query.Where(f => f.AssignedUserId == assignedUserId.Value);
                }

                if (customerId.HasValue)
                {
                    query = query.Where(f => f.CustomerId == customerId.Value);
                }

                if (isOverdue.HasValue && isOverdue.Value)
                {
                    var sevenDaysAgo = DateTime.Now.AddDays(-7);
                    query = query.Where(f => f.Status != FeedbackStatus.Resolved && f.CreatedAt < sevenDaysAgo);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var feedbacks = await query
                    .OrderBy(f => f.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var feedbackDtos = feedbacks.Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Description = f.Description,
                    FeedbackType = f.FeedbackType,
                    Status = f.Status,
                    Priority = f.Priority,
                    CustomerId = f.CustomerId,
                    CustomerName = f.Customer?.FullName,
                    CustomerEmail = f.Customer?.Email,
                    CustomerPhone = f.Customer?.PhoneNumber,
                    AssignedUserId = f.AssignedUserId,
                    AssignedUserName = f.AssignedUser?.FullName,
                    OrderId = f.OrderId,
                    OrderCode = f.Order?.OrderNumber,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    ResolvedAt = f.ResolvedAt,
                    Resolution = f.Resolution,
                    Notes = f.Notes
                }).ToList();

                return ServiceResponse<List<FeedbackDto>>.SuccessResponse(feedbackDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedbacks");
                return ServiceResponse<List<FeedbackDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách phản hồi");
            }
        }

        public async Task<ServiceResponse<FeedbackDto>> GetFeedbackByIdAsync(Guid id)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.Customer)
                    .Include(f => f.AssignedUser)
                    .Include(f => f.Order)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (feedback == null)
                {
                    return ServiceResponse<FeedbackDto>.ErrorResponse("Không tìm thấy phản hồi");
                }

                var dto = new FeedbackDto
                {
                    Id = feedback.Id,
                    Title = feedback.Title,
                    Description = feedback.Description,
                    FeedbackType = feedback.FeedbackType,
                    Status = feedback.Status,
                    Priority = feedback.Priority,
                    CustomerId = feedback.CustomerId,
                    CustomerName = feedback.Customer?.FullName,
                    CustomerEmail = feedback.Customer?.Email,
                    CustomerPhone = feedback.Customer?.PhoneNumber,
                    AssignedUserId = feedback.AssignedUserId,
                    AssignedUserName = feedback.AssignedUser?.FullName,
                    OrderId = feedback.OrderId,
                    OrderCode = feedback.Order?.OrderNumber,
                    CreatedAt = feedback.CreatedAt,
                    UpdatedAt = feedback.UpdatedAt,
                    ResolvedAt = feedback.ResolvedAt,
                    Resolution = feedback.Resolution,
                    Notes = feedback.Notes
                };

                return ServiceResponse<FeedbackDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback by ID");
                return ServiceResponse<FeedbackDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<FeedbackDto>> CreateFeedbackAsync(CreateFeedbackDto dto)
        {
            try
            {
                // Create new feedback
                var feedback = new Data.Entities.Feedback
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    FeedbackType = dto.FeedbackType,
                    Priority = dto.Priority,
                    Status = FeedbackStatus.Open,
                    CustomerId = dto.CustomerId ?? Guid.Empty, // Handle nullable Guid
                    OrderId = dto.OrderId,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                // Return the created feedback
                var result = await GetFeedbackByIdAsync(feedback.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback");
                return ServiceResponse<FeedbackDto>.ErrorResponse("Đã xảy ra lỗi khi tạo phản hồi");
            }
        }

        public async Task<ServiceResponse<FeedbackDto>> UpdateFeedbackAsync(UpdateFeedbackDto dto)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == dto.Id);

                if (feedback == null)
                {
                    return ServiceResponse<FeedbackDto>.ErrorResponse("Không tìm thấy phản hồi");
                }

                // Update feedback properties
                feedback.Title = dto.Title;
                feedback.Description = dto.Description;
                feedback.FeedbackType = dto.FeedbackType;
                feedback.Priority = dto.Priority;
                feedback.Status = dto.Status;
                feedback.AssignedUserId = dto.AssignedUserId;
                feedback.Resolution = dto.Resolution;
                feedback.Notes = dto.Notes;
                feedback.UpdatedAt = DateTime.UtcNow;

                // Set resolved date if status is resolved
                if (dto.Status == FeedbackStatus.Resolved && feedback.ResolvedAt == null)
                {
                    feedback.ResolvedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Return the updated feedback
                var result = await GetFeedbackByIdAsync(feedback.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback");
                return ServiceResponse<FeedbackDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật phản hồi");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteFeedbackAsync(Guid id)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (feedback == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy phản hồi");
                }

                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xóa phản hồi");
            }
        }

        public async Task<ServiceResponse<FeedbackDto>> AssignFeedbackAsync(Guid feedbackId, Guid assignedUserId)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == feedbackId);

                if (feedback == null)
                {
                    return ServiceResponse<FeedbackDto>.ErrorResponse("Không tìm thấy phản hồi");
                }

                feedback.AssignedUserId = assignedUserId;
                feedback.Status = FeedbackStatus.InProgress;
                feedback.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = await GetFeedbackByIdAsync(feedback.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning feedback");
                return ServiceResponse<FeedbackDto>.ErrorResponse("Đã xảy ra lỗi khi phân công phản hồi");
            }
        }

        public async Task<ServiceResponse<FeedbackDto>> ResolveFeedbackAsync(Guid feedbackId, string resolution)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == feedbackId);

                if (feedback == null)
                {
                    return ServiceResponse<FeedbackDto>.ErrorResponse("Không tìm thấy phản hồi");
                }

                feedback.Status = FeedbackStatus.Resolved;
                feedback.Resolution = resolution;
                feedback.ResolvedAt = DateTime.UtcNow;
                feedback.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = await GetFeedbackByIdAsync(feedback.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving feedback");
                return ServiceResponse<FeedbackDto>.ErrorResponse("Đã xảy ra lỗi khi giải quyết phản hồi");
            }
        }

        public async Task<ServiceResponse<List<FeedbackSummaryDto>>> GetFeedbackSummaryAsync(
            FeedbackStatus? status = null,
            Guid? assignedUserId = null,
            bool? isOverdue = null)
        {
            try
            {
                var query = _context.Feedbacks
                    .Include(f => f.Customer)
                    .Include(f => f.AssignedUser)
                    .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(f => f.Status == status.Value);
                }

                if (assignedUserId.HasValue)
                {
                    query = query.Where(f => f.AssignedUserId == assignedUserId.Value);
                }

                if (isOverdue.HasValue && isOverdue.Value)
                {
                    var sevenDaysAgo = DateTime.Now.AddDays(-7);
                    query = query.Where(f => f.Status != FeedbackStatus.Resolved && f.CreatedAt < sevenDaysAgo);
                }

                var feedbacks = await query.ToListAsync();

                var summaryDtos = feedbacks.Select(f => new FeedbackSummaryDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    FeedbackType = f.FeedbackType,
                    Status = f.Status,
                    Priority = f.Priority,
                    CustomerName = f.Customer?.FullName,
                    AssignedUserName = f.AssignedUser?.FullName,
                    CreatedAt = f.CreatedAt,
                    DaysOpen = (DateTime.Now - f.CreatedAt).Days,
                    IsOverdue = f.Status != FeedbackStatus.Resolved && (DateTime.Now - f.CreatedAt).Days > 7
                }).ToList();

                return ServiceResponse<List<FeedbackSummaryDto>>.SuccessResponse(summaryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback summary");
                return ServiceResponse<List<FeedbackSummaryDto>>.ErrorResponse("Đã xảy ra lỗi khi tải tóm tắt phản hồi");
            }
        }

        public async Task<ServiceResponse<List<FeedbackDto>>> GetMyFeedbacksAsync(Guid userId)
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Customer)
                    .Include(f => f.AssignedUser)
                    .Include(f => f.Order)
                    .Where(f => f.AssignedUserId == userId)
                    .OrderBy(f => f.CreatedAt)
                    .ToListAsync();

                var feedbackDtos = feedbacks.Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Description = f.Description,
                    FeedbackType = f.FeedbackType,
                    Status = f.Status,
                    Priority = f.Priority,
                    CustomerId = f.CustomerId,
                    CustomerName = f.Customer?.FullName,
                    CustomerEmail = f.Customer?.Email,
                    CustomerPhone = f.Customer?.PhoneNumber,
                    AssignedUserId = f.AssignedUserId,
                    AssignedUserName = f.AssignedUser?.FullName,
                    OrderId = f.OrderId,
                    OrderCode = f.Order?.OrderNumber,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    ResolvedAt = f.ResolvedAt,
                    Resolution = f.Resolution,
                    Notes = f.Notes
                }).ToList();

                return ServiceResponse<List<FeedbackDto>>.SuccessResponse(feedbackDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my feedbacks");
                return ServiceResponse<List<FeedbackDto>>.ErrorResponse("Đã xảy ra lỗi khi tải phản hồi của tôi");
            }
        }

        public async Task<ServiceResponse<List<FeedbackDto>>> GetOverdueFeedbacksAsync()
        {
            try
            {
                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Customer)
                    .Include(f => f.AssignedUser)
                    .Include(f => f.Order)
                    .Where(f => f.Status != FeedbackStatus.Resolved && f.CreatedAt < sevenDaysAgo)
                    .OrderBy(f => f.CreatedAt)
                    .ToListAsync();

                var feedbackDtos = feedbacks.Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Description = f.Description,
                    FeedbackType = f.FeedbackType,
                    Status = f.Status,
                    Priority = f.Priority,
                    CustomerId = f.CustomerId,
                    CustomerName = f.Customer?.FullName,
                    CustomerEmail = f.Customer?.Email,
                    CustomerPhone = f.Customer?.PhoneNumber,
                    AssignedUserId = f.AssignedUserId,
                    AssignedUserName = f.AssignedUser?.FullName,
                    OrderId = f.OrderId,
                    OrderCode = f.Order?.OrderNumber,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    ResolvedAt = f.ResolvedAt,
                    Resolution = f.Resolution,
                    Notes = f.Notes
                }).ToList();

                return ServiceResponse<List<FeedbackDto>>.SuccessResponse(feedbackDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue feedbacks");
                return ServiceResponse<List<FeedbackDto>>.ErrorResponse("Đã xảy ra lỗi khi tải phản hồi quá hạn");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalFeedbacksCountAsync()
        {
            try
            {
                var count = await _context.Feedbacks.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total feedbacks count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetOpenFeedbacksCountAsync()
        {
            try
            {
                var count = await _context.Feedbacks
                    .CountAsync(f => f.Status != FeedbackStatus.Resolved);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting open feedbacks count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetOverdueFeedbacksCountAsync()
        {
            try
            {
                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                var count = await _context.Feedbacks
                    .CountAsync(f => f.Status != FeedbackStatus.Resolved && f.CreatedAt < sevenDaysAgo);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue feedbacks count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetAverageResolutionTimeAsync()
        {
            try
            {
                var resolvedFeedbacks = await _context.Feedbacks
                    .Where(f => f.Status == FeedbackStatus.Resolved && f.ResolvedAt.HasValue)
                    .ToListAsync();

                if (!resolvedFeedbacks.Any())
                {
                    return ServiceResponse<decimal>.SuccessResponse(0);
                }

                var averageDays = resolvedFeedbacks
                    .Average(f => (f.ResolvedAt!.Value - f.CreatedAt).TotalDays);

                return ServiceResponse<decimal>.SuccessResponse((decimal)averageDays);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average resolution time");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<List<FeedbackDto>>> GetRecentFeedbacksAsync(int count = 10)
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Customer)
                    .Include(f => f.AssignedUser)
                    .Include(f => f.Order)
                    .OrderBy(f => f.CreatedAt)
                    .Take(count)
                    .ToListAsync();

                var feedbackDtos = feedbacks.Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Description = f.Description,
                    FeedbackType = f.FeedbackType,
                    Status = f.Status,
                    Priority = f.Priority,
                    CustomerId = f.CustomerId,
                    CustomerName = f.Customer?.FullName,
                    CustomerEmail = f.Customer?.Email,
                    CustomerPhone = f.Customer?.PhoneNumber,
                    AssignedUserId = f.AssignedUserId,
                    AssignedUserName = f.AssignedUser?.FullName,
                    OrderId = f.OrderId,
                    OrderCode = f.Order?.OrderNumber,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    ResolvedAt = f.ResolvedAt,
                    Resolution = f.Resolution,
                    Notes = f.Notes
                }).ToList();

                return ServiceResponse<List<FeedbackDto>>.SuccessResponse(feedbackDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent feedbacks");
                return ServiceResponse<List<FeedbackDto>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }
    }
}
