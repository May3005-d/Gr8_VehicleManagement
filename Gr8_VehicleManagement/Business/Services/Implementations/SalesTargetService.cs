using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class SalesTargetService : ISalesTargetService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SalesTargetService> _logger;

        public SalesTargetService(ApplicationDbContext context, ILogger<SalesTargetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<SalesTargetDto>>> GetAllSalesTargetsAsync(
            string? searchTerm = null,
            TargetFor? targetFor = null,
            Guid? dealerId = null,
            Guid? userId = null,
            bool? isActive = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.SalesTargets
                    .Include(st => st.Dealer)
                    .Include(st => st.User)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(st => 
                        st.Name.Contains(searchTerm) || 
                        (st.Description != null && st.Description.Contains(searchTerm)));
                }

                if (targetFor.HasValue)
                {
                    query = query.Where(st => st.TargetFor == targetFor.Value);
                }

                if (dealerId.HasValue)
                {
                    query = query.Where(st => st.DealerId == dealerId.Value);
                }

                if (userId.HasValue)
                {
                    query = query.Where(st => st.UserId == userId.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(st => st.IsActive == isActive.Value);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(st => st.StartDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(st => st.EndDate <= endDate.Value);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var salesTargets = await query
                    .OrderBy(st => st.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var salesTargetDtos = salesTargets.Select(st => new SalesTargetDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description ?? string.Empty,
                    TargetFor = st.TargetFor,
                    DealerId = st.DealerId,
                    DealerName = st.Dealer?.Name,
                    UserId = st.UserId,
                    UserName = st.User?.FullName,
                    TargetAmount = st.TargetAmount,
                    TargetQuantity = st.TargetQuantity,
                    StartDate = st.StartDate,
                    EndDate = st.EndDate,
                    AchievementAmount = st.AchievementAmount,
                    AchievementQuantity = st.AchievementQuantity,
                    CommissionRate = st.CommissionRate,
                    CommissionEarned = st.CommissionEarned,
                    IsActive = st.IsActive,
                    CreatedAt = st.CreatedAt,
                    UpdatedAt = st.UpdatedAt
                }).ToList();

                return ServiceResponse<List<SalesTargetDto>>.SuccessResponse(salesTargetDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales targets");
                return ServiceResponse<List<SalesTargetDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách chỉ tiêu bán hàng");
            }
        }

        public async Task<ServiceResponse<SalesTargetDto>> GetSalesTargetByIdAsync(Guid id)
        {
            try
            {
                var salesTarget = await _context.SalesTargets
                    .Include(st => st.Dealer)
                    .Include(st => st.User)
                    .FirstOrDefaultAsync(st => st.Id == id);

                if (salesTarget == null)
                {
                    return ServiceResponse<SalesTargetDto>.ErrorResponse("Không tìm thấy chỉ tiêu bán hàng");
                }

                var dto = new SalesTargetDto
                {
                    Id = salesTarget.Id,
                    Name = salesTarget.Name,
                    Description = salesTarget.Description ?? string.Empty,
                    TargetFor = salesTarget.TargetFor,
                    DealerId = salesTarget.DealerId,
                    DealerName = salesTarget.Dealer?.Name,
                    UserId = salesTarget.UserId,
                    UserName = salesTarget.User?.FullName,
                    TargetAmount = salesTarget.TargetAmount,
                    TargetQuantity = salesTarget.TargetQuantity,
                    StartDate = salesTarget.StartDate,
                    EndDate = salesTarget.EndDate,
                    AchievementAmount = salesTarget.AchievementAmount,
                    AchievementQuantity = salesTarget.AchievementQuantity,
                    CommissionRate = salesTarget.CommissionRate,
                    CommissionEarned = salesTarget.CommissionEarned,
                    IsActive = salesTarget.IsActive,
                    CreatedAt = salesTarget.CreatedAt,
                    UpdatedAt = salesTarget.UpdatedAt
                };

                return ServiceResponse<SalesTargetDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales target by ID");
                return ServiceResponse<SalesTargetDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<SalesTargetDto>> CreateSalesTargetAsync(CreateSalesTargetDto dto)
        {
            try
            {
                // Validate dates
                if (dto.StartDate >= dto.EndDate)
                {
                    return ServiceResponse<SalesTargetDto>.ErrorResponse("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                }

                // Validate target for
                if (dto.TargetFor == TargetFor.Dealer && !dto.DealerId.HasValue)
                {
                    return ServiceResponse<SalesTargetDto>.ErrorResponse("Phải chọn đại lý khi chỉ tiêu dành cho đại lý");
                }

                if (dto.TargetFor == TargetFor.Staff && !dto.UserId.HasValue)
                {
                    return ServiceResponse<SalesTargetDto>.ErrorResponse("Phải chọn nhân viên khi chỉ tiêu dành cho nhân viên");
                }

                // Create new sales target
                var salesTarget = new Data.Entities.SalesTarget
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    TargetFor = dto.TargetFor,
                    DealerId = dto.DealerId,
                    UserId = dto.UserId,
                    TargetAmount = dto.TargetAmount,
                    TargetQuantity = dto.TargetQuantity,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    AchievementAmount = 0,
                    AchievementQuantity = 0,
                    CommissionRate = dto.CommissionRate,
                    CommissionEarned = 0,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.SalesTargets.Add(salesTarget);
                await _context.SaveChangesAsync();

                // Return the created sales target
                var result = await GetSalesTargetByIdAsync(salesTarget.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sales target");
                return ServiceResponse<SalesTargetDto>.ErrorResponse("Đã xảy ra lỗi khi tạo chỉ tiêu bán hàng");
            }
        }

        public async Task<ServiceResponse<SalesTargetDto>> UpdateSalesTargetAsync(UpdateSalesTargetDto dto)
        {
            try
            {
                var salesTarget = await _context.SalesTargets
                    .FirstOrDefaultAsync(st => st.Id == dto.Id);

                if (salesTarget == null)
                {
                    return ServiceResponse<SalesTargetDto>.ErrorResponse("Không tìm thấy chỉ tiêu bán hàng");
                }

                // Validate dates
                if (dto.StartDate >= dto.EndDate)
                {
                    return ServiceResponse<SalesTargetDto>.ErrorResponse("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                }

                // Update sales target properties
                salesTarget.Name = dto.Name;
                salesTarget.Description = dto.Description;
                salesTarget.TargetFor = dto.TargetFor;
                salesTarget.DealerId = dto.DealerId;
                salesTarget.UserId = dto.UserId;
                salesTarget.TargetAmount = dto.TargetAmount;
                salesTarget.TargetQuantity = dto.TargetQuantity;
                salesTarget.StartDate = dto.StartDate;
                salesTarget.EndDate = dto.EndDate;
                salesTarget.CommissionRate = dto.CommissionRate;
                salesTarget.IsActive = dto.IsActive;
                salesTarget.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Return the updated sales target
                var result = await GetSalesTargetByIdAsync(salesTarget.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sales target");
                return ServiceResponse<SalesTargetDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật chỉ tiêu bán hàng");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteSalesTargetAsync(Guid id)
        {
            try
            {
                var salesTarget = await _context.SalesTargets
                    .FirstOrDefaultAsync(st => st.Id == id);

                if (salesTarget == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy chỉ tiêu bán hàng");
                }

                _context.SalesTargets.Remove(salesTarget);
                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sales target");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xóa chỉ tiêu bán hàng");
            }
        }

        public async Task<ServiceResponse<List<SalesTargetSummaryDto>>> GetSalesTargetSummaryAsync(
            Guid? dealerId = null,
            Guid? userId = null,
            bool? isActive = null)
        {
            try
            {
                var query = _context.SalesTargets
                    .Include(st => st.Dealer)
                    .Include(st => st.User)
                    .AsQueryable();

                if (dealerId.HasValue)
                {
                    query = query.Where(st => st.DealerId == dealerId.Value);
                }

                if (userId.HasValue)
                {
                    query = query.Where(st => st.UserId == userId.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(st => st.IsActive == isActive.Value);
                }

                var salesTargets = await query.ToListAsync();

                var summaryDtos = salesTargets.Select(st => new SalesTargetSummaryDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    TargetForDisplay = st.TargetFor.ToString(),
                    DealerName = st.Dealer?.Name,
                    UserName = st.User?.FullName,
                    TargetAmount = st.TargetAmount,
                    AchievementAmount = st.AchievementAmount,
                    AchievementPercentage = st.TargetAmount > 0 ? (st.AchievementAmount / st.TargetAmount) * 100 : 0,
                    CommissionEarned = st.CommissionEarned,
                    IsActive = st.IsActive,
                    EndDate = st.EndDate
                }).ToList();

                return ServiceResponse<List<SalesTargetSummaryDto>>.SuccessResponse(summaryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales target summary");
                return ServiceResponse<List<SalesTargetSummaryDto>>.ErrorResponse("Đã xảy ra lỗi khi tải tóm tắt chỉ tiêu");
            }
        }

        public async Task<ServiceResponse<decimal>> CalculateCommissionAsync(Guid salesTargetId, decimal achievementAmount)
        {
            try
            {
                var salesTarget = await _context.SalesTargets
                    .FirstOrDefaultAsync(st => st.Id == salesTargetId);

                if (salesTarget == null)
                {
                    return ServiceResponse<decimal>.ErrorResponse("Không tìm thấy chỉ tiêu bán hàng");
                }

                var commission = achievementAmount * (salesTarget.CommissionRate / 100);
                return ServiceResponse<decimal>.SuccessResponse(commission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating commission");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi khi tính hoa hồng");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAchievementAsync(Guid salesTargetId, decimal amount, int quantity)
        {
            try
            {
                var salesTarget = await _context.SalesTargets
                    .FirstOrDefaultAsync(st => st.Id == salesTargetId);

                if (salesTarget == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy chỉ tiêu bán hàng");
                }

                // Update achievement
                salesTarget.AchievementAmount += amount;
                salesTarget.AchievementQuantity += quantity;

                // Calculate commission
                var commission = amount * (salesTarget.CommissionRate / 100);
                salesTarget.CommissionEarned += commission;

                salesTarget.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating achievement");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi cập nhật thành tích");
            }
        }

        public async Task<ServiceResponse<List<SalesTargetDto>>> GetActiveTargetsForUserAsync(Guid userId)
        {
            try
            {
                var now = DateTime.Now;
                var salesTargets = await _context.SalesTargets
                    .Include(st => st.Dealer)
                    .Include(st => st.User)
                    .Where(st => st.UserId == userId && 
                                st.IsActive && 
                                st.StartDate <= now && 
                                st.EndDate >= now)
                    .ToListAsync();

                var salesTargetDtos = salesTargets.Select(st => new SalesTargetDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description ?? string.Empty,
                    TargetFor = st.TargetFor,
                    DealerId = st.DealerId,
                    DealerName = st.Dealer?.Name,
                    UserId = st.UserId,
                    UserName = st.User?.FullName,
                    TargetAmount = st.TargetAmount,
                    TargetQuantity = st.TargetQuantity,
                    StartDate = st.StartDate,
                    EndDate = st.EndDate,
                    AchievementAmount = st.AchievementAmount,
                    AchievementQuantity = st.AchievementQuantity,
                    CommissionRate = st.CommissionRate,
                    CommissionEarned = st.CommissionEarned,
                    IsActive = st.IsActive,
                    CreatedAt = st.CreatedAt,
                    UpdatedAt = st.UpdatedAt
                }).ToList();

                return ServiceResponse<List<SalesTargetDto>>.SuccessResponse(salesTargetDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active targets for user");
                return ServiceResponse<List<SalesTargetDto>>.ErrorResponse("Đã xảy ra lỗi khi tải chỉ tiêu hoạt động");
            }
        }

        public async Task<ServiceResponse<List<SalesTargetDto>>> GetActiveTargetsForDealerAsync(Guid dealerId)
        {
            try
            {
                var now = DateTime.Now;
                var salesTargets = await _context.SalesTargets
                    .Include(st => st.Dealer)
                    .Include(st => st.User)
                    .Where(st => st.DealerId == dealerId && 
                                st.IsActive && 
                                st.StartDate <= now && 
                                st.EndDate >= now)
                    .ToListAsync();

                var salesTargetDtos = salesTargets.Select(st => new SalesTargetDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description ?? string.Empty,
                    TargetFor = st.TargetFor,
                    DealerId = st.DealerId,
                    DealerName = st.Dealer?.Name,
                    UserId = st.UserId,
                    UserName = st.User?.FullName,
                    TargetAmount = st.TargetAmount,
                    TargetQuantity = st.TargetQuantity,
                    StartDate = st.StartDate,
                    EndDate = st.EndDate,
                    AchievementAmount = st.AchievementAmount,
                    AchievementQuantity = st.AchievementQuantity,
                    CommissionRate = st.CommissionRate,
                    CommissionEarned = st.CommissionEarned,
                    IsActive = st.IsActive,
                    CreatedAt = st.CreatedAt,
                    UpdatedAt = st.UpdatedAt
                }).ToList();

                return ServiceResponse<List<SalesTargetDto>>.SuccessResponse(salesTargetDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active targets for dealer");
                return ServiceResponse<List<SalesTargetDto>>.ErrorResponse("Đã xảy ra lỗi khi tải chỉ tiêu hoạt động");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalSalesTargetsCountAsync()
        {
            try
            {
                var count = await _context.SalesTargets.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total sales targets count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetActiveSalesTargetsCountAsync()
        {
            try
            {
                var now = DateTime.Now;
                var count = await _context.SalesTargets
                    .CountAsync(st => st.IsActive && st.StartDate <= now && st.EndDate >= now);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sales targets count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetTotalCommissionEarnedAsync(Guid? dealerId = null, Guid? userId = null)
        {
            try
            {
                var query = _context.SalesTargets.AsQueryable();

                if (dealerId.HasValue)
                {
                    query = query.Where(st => st.DealerId == dealerId.Value);
                }

                if (userId.HasValue)
                {
                    query = query.Where(st => st.UserId == userId.Value);
                }

                var totalCommission = await query.SumAsync(st => st.CommissionEarned);
                return ServiceResponse<decimal>.SuccessResponse(totalCommission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total commission earned");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<List<SalesTargetDto>>> GetTopPerformersAsync(int count = 10)
        {
            try
            {
                var salesTargets = await _context.SalesTargets
                    .Include(st => st.Dealer)
                    .Include(st => st.User)
                    .OrderByDescending(st => st.TargetAmount > 0 ? (st.AchievementAmount / st.TargetAmount) * 100 : 0)
                    .Take(count)
                    .ToListAsync();

                var salesTargetDtos = salesTargets.Select(st => new SalesTargetDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description ?? string.Empty,
                    TargetFor = st.TargetFor,
                    DealerId = st.DealerId,
                    DealerName = st.Dealer?.Name,
                    UserId = st.UserId,
                    UserName = st.User?.FullName,
                    TargetAmount = st.TargetAmount,
                    TargetQuantity = st.TargetQuantity,
                    StartDate = st.StartDate,
                    EndDate = st.EndDate,
                    AchievementAmount = st.AchievementAmount,
                    AchievementQuantity = st.AchievementQuantity,
                    CommissionRate = st.CommissionRate,
                    CommissionEarned = st.CommissionEarned,
                    IsActive = st.IsActive,
                    CreatedAt = st.CreatedAt,
                    UpdatedAt = st.UpdatedAt
                }).ToList();

                return ServiceResponse<List<SalesTargetDto>>.SuccessResponse(salesTargetDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top performers");
                return ServiceResponse<List<SalesTargetDto>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }
    }
}