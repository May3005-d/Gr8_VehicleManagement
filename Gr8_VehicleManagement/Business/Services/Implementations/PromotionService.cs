using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(ApplicationDbContext context, ILogger<PromotionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<PromotionDto>>> GetAllPromotionsAsync(
            string? searchTerm = null,
            bool? isActive = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Promotions.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => 
                        p.Name.Contains(searchTerm) || 
                        p.Code.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm)));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == isActive.Value);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(p => p.StartDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(p => p.EndDate <= endDate.Value);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var promotions = await query
                    .OrderBy(p => p.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var promotionDtos = promotions.Select(p => new PromotionDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    DiscountType = p.DiscountType,
                    DiscountValue = p.DiscountValue,
                    MinOrderValue = p.MinOrderAmount,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    UsageLimit = p.Quota,
                    UsedCount = p.UsedCount,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    TargetAudience = p.ApplicableDealers,
                    ApplicableVehicles = p.ApplicableModels,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return ServiceResponse<List<PromotionDto>>.SuccessResponse(promotionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting promotions");
                return ServiceResponse<List<PromotionDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách khuyến mãi");
            }
        }

        public async Task<ServiceResponse<PromotionDto>> GetPromotionByIdAsync(Guid id)
        {
            try
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (promotion == null)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Không tìm thấy khuyến mãi");
                }

                var dto = new PromotionDto
                {
                    Id = promotion.Id,
                    Code = promotion.Code,
                    Name = promotion.Name,
                    Description = promotion.Description ?? string.Empty,
                    DiscountType = promotion.DiscountType,
                    DiscountValue = promotion.DiscountValue,
                    MinOrderValue = promotion.MinOrderAmount,
                    MaxDiscountAmount = promotion.MaxDiscountAmount,
                    UsageLimit = promotion.Quota,
                    UsedCount = promotion.UsedCount,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    IsActive = promotion.IsActive,
                    TargetAudience = promotion.ApplicableDealers,
                    ApplicableVehicles = promotion.ApplicableModels,
                    CreatedAt = promotion.CreatedAt,
                    UpdatedAt = promotion.UpdatedAt
                };

                return ServiceResponse<PromotionDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting promotion by ID");
                return ServiceResponse<PromotionDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<PromotionDto>> GetPromotionByCodeAsync(string code)
        {
            try
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Code == code);

                if (promotion == null)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Không tìm thấy mã khuyến mãi");
                }

                var dto = new PromotionDto
                {
                    Id = promotion.Id,
                    Code = promotion.Code,
                    Name = promotion.Name,
                    Description = promotion.Description ?? string.Empty,
                    DiscountType = promotion.DiscountType,
                    DiscountValue = promotion.DiscountValue,
                    MinOrderValue = promotion.MinOrderAmount,
                    MaxDiscountAmount = promotion.MaxDiscountAmount,
                    UsageLimit = promotion.Quota,
                    UsedCount = promotion.UsedCount,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    IsActive = promotion.IsActive,
                    TargetAudience = promotion.ApplicableDealers,
                    ApplicableVehicles = promotion.ApplicableModels,
                    CreatedAt = promotion.CreatedAt,
                    UpdatedAt = promotion.UpdatedAt
                };

                return ServiceResponse<PromotionDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting promotion by code");
                return ServiceResponse<PromotionDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<PromotionDto>> CreatePromotionAsync(CreatePromotionDto dto)
        {
            try
            {
                // Check if code already exists
                var existingPromotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Code == dto.Code);
                
                if (existingPromotion != null)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Mã khuyến mãi đã tồn tại");
                }

                // Validate dates
                if (dto.StartDate >= dto.EndDate)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                }

                // Create new promotion
                var promotion = new Data.Entities.Promotion
                {
                    Id = Guid.NewGuid(),
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    DiscountType = dto.DiscountType,
                    DiscountValue = dto.DiscountValue,
                    MinOrderAmount = dto.MinOrderValue,
                    MaxDiscountAmount = dto.MaxDiscountAmount,
                    Quota = dto.UsageLimit,
                    UsedCount = 0,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    ApplicableDealers = dto.TargetAudience,
                    ApplicableModels = dto.ApplicableVehicles,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();

                // Return the created promotion
                var result = await GetPromotionByIdAsync(promotion.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating promotion");
                return ServiceResponse<PromotionDto>.ErrorResponse("Đã xảy ra lỗi khi tạo khuyến mãi");
            }
        }

        public async Task<ServiceResponse<PromotionDto>> UpdatePromotionAsync(UpdatePromotionDto dto)
        {
            try
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == dto.Id);

                if (promotion == null)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Không tìm thấy khuyến mãi");
                }

                // Check if code already exists (excluding current promotion)
                var existingPromotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Code == dto.Code && p.Id != dto.Id);
                
                if (existingPromotion != null)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Mã khuyến mãi đã tồn tại");
                }

                // Validate dates
                if (dto.StartDate >= dto.EndDate)
                {
                    return ServiceResponse<PromotionDto>.ErrorResponse("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                }

                // Update promotion properties
                promotion.Code = dto.Code;
                promotion.Name = dto.Name;
                promotion.Description = dto.Description;
                promotion.DiscountType = dto.DiscountType;
                promotion.DiscountValue = dto.DiscountValue;
                promotion.MinOrderAmount = dto.MinOrderValue;
                promotion.MaxDiscountAmount = dto.MaxDiscountAmount;
                promotion.Quota = dto.UsageLimit;
                promotion.StartDate = dto.StartDate;
                promotion.EndDate = dto.EndDate;
                promotion.IsActive = dto.IsActive;
                promotion.ApplicableDealers = dto.TargetAudience;
                promotion.ApplicableModels = dto.ApplicableVehicles;
                promotion.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Return the updated promotion
                var result = await GetPromotionByIdAsync(promotion.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating promotion");
                return ServiceResponse<PromotionDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật khuyến mãi");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePromotionAsync(Guid id)
        {
            try
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (promotion == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy khuyến mãi");
                }

                // Check if promotion has been used
                if (promotion.UsedCount > 0)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không thể xóa khuyến mãi đã được sử dụng");
                }

                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting promotion");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xóa khuyến mãi");
            }
        }

        public async Task<ServiceResponse<PromotionApplicationResult>> ApplyPromotionAsync(ApplyPromotionDto dto)
        {
            try
            {
                // Get promotion by code
                var promotionResult = await GetPromotionByCodeAsync(dto.PromotionCode);
                if (!promotionResult.Success || promotionResult.Data == null)
                {
                    return ServiceResponse<PromotionApplicationResult>.ErrorResponse("Mã khuyến mãi không hợp lệ");
                }

                var promotion = promotionResult.Data;

                // Validate promotion
                if (!promotion.IsValid)
                {
                    return ServiceResponse<PromotionApplicationResult>.ErrorResponse("Khuyến mãi không còn hiệu lực");
                }

                // Check minimum order value
                if (promotion.MinOrderValue.HasValue && dto.OrderValue < promotion.MinOrderValue.Value)
                {
                    return ServiceResponse<PromotionApplicationResult>.ErrorResponse(
                        $"Đơn hàng phải có giá trị tối thiểu {promotion.MinOrderValue.Value:N0} VNĐ");
                }

                // Calculate discount
                decimal discountAmount = 0;
                if (promotion.DiscountType == DiscountType.Percentage)
                {
                    discountAmount = dto.OrderValue * (promotion.DiscountValue / 100);
                }
                else
                {
                    discountAmount = promotion.DiscountValue;
                }

                // Apply maximum discount limit
                if (promotion.MaxDiscountAmount.HasValue && discountAmount > promotion.MaxDiscountAmount.Value)
                {
                    discountAmount = promotion.MaxDiscountAmount.Value;
                }

                var finalAmount = dto.OrderValue - discountAmount;

                var result = new PromotionApplicationResult
                {
                    Success = true,
                    Message = "Áp dụng khuyến mãi thành công",
                    DiscountAmount = discountAmount,
                    FinalAmount = finalAmount,
                    AppliedPromotion = promotion
                };

                return ServiceResponse<PromotionApplicationResult>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying promotion");
                return ServiceResponse<PromotionApplicationResult>.ErrorResponse("Đã xảy ra lỗi khi áp dụng khuyến mãi");
            }
        }

        public async Task<ServiceResponse<List<PromotionDto>>> GetApplicablePromotionsAsync(
            decimal orderValue,
            List<Guid> vehicleIds,
            Guid? customerId = null)
        {
            try
            {
                var now = DateTime.Now;
                var query = _context.Promotions
                    .Where(p => p.IsActive && 
                               p.StartDate <= now && 
                               p.EndDate >= now &&
                               (p.Quota == null || p.UsedCount < p.Quota));

                // Filter by minimum order value
                query = query.Where(p => p.MinOrderAmount == null || p.MinOrderAmount <= orderValue);

                var promotions = await query.ToListAsync();

                var applicablePromotions = new List<PromotionDto>();
                foreach (var promotion in promotions)
                {
                    var dto = new PromotionDto
                    {
                        Id = promotion.Id,
                        Code = promotion.Code,
                        Name = promotion.Name,
                        Description = promotion.Description ?? string.Empty,
                        DiscountType = promotion.DiscountType,
                        DiscountValue = promotion.DiscountValue,
                        MinOrderValue = promotion.MinOrderAmount,
                        MaxDiscountAmount = promotion.MaxDiscountAmount,
                        UsageLimit = promotion.Quota,
                        UsedCount = promotion.UsedCount,
                        StartDate = promotion.StartDate,
                        EndDate = promotion.EndDate,
                        IsActive = promotion.IsActive,
                        TargetAudience = promotion.ApplicableDealers,
                        ApplicableVehicles = promotion.ApplicableModels,
                        CreatedAt = promotion.CreatedAt,
                        UpdatedAt = promotion.UpdatedAt
                    };

                    if (dto.IsValid)
                    {
                        applicablePromotions.Add(dto);
                    }
                }

                return ServiceResponse<List<PromotionDto>>.SuccessResponse(applicablePromotions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting applicable promotions");
                return ServiceResponse<List<PromotionDto>>.ErrorResponse("Đã xảy ra lỗi khi tải khuyến mãi áp dụng");
            }
        }

        public async Task<ServiceResponse<bool>> ValidatePromotionAsync(string code, decimal orderValue, List<Guid> vehicleIds, Guid? customerId)
        {
            try
            {
                var promotionResult = await GetPromotionByCodeAsync(code);
                if (!promotionResult.Success || promotionResult.Data == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Mã khuyến mãi không hợp lệ");
                }

                var promotion = promotionResult.Data;

                // Check if promotion is valid
                if (!promotion.IsValid)
                {
                    return ServiceResponse<bool>.ErrorResponse("Khuyến mãi không còn hiệu lực");
                }

                // Check minimum order value
                if (promotion.MinOrderValue.HasValue && orderValue < promotion.MinOrderValue.Value)
                {
                    return ServiceResponse<bool>.ErrorResponse(
                        $"Đơn hàng phải có giá trị tối thiểu {promotion.MinOrderValue.Value:N0} VNĐ");
                }

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating promotion");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xác thực khuyến mãi");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalPromotionsCountAsync()
        {
            try
            {
                var count = await _context.Promotions.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total promotions count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetActivePromotionsCountAsync()
        {
            try
            {
                var now = DateTime.Now;
                var count = await _context.Promotions
                    .CountAsync(p => p.IsActive && p.StartDate <= now && p.EndDate >= now);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active promotions count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public Task<ServiceResponse<decimal>> GetTotalDiscountGivenAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // This would require tracking discount applications in a separate table
                // For now, return 0 as we don't have this tracking yet
                return Task.FromResult(ServiceResponse<decimal>.SuccessResponse(0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total discount given");
                return Task.FromResult(ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi"));
            }
        }

        public async Task<ServiceResponse<bool>> IncrementPromotionUsageAsync(Guid promotionId)
        {
            try
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == promotionId);

                if (promotion == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy khuyến mãi");
                }

                promotion.UsedCount++;
                promotion.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing promotion usage");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi cập nhật số lần sử dụng");
            }
        }

        public async Task<ServiceResponse<List<PromotionDto>>> GetMostUsedPromotionsAsync(int count = 10)
        {
            try
            {
                var promotions = await _context.Promotions
                    .OrderByDescending(p => p.UsedCount)
                    .Take(count)
                    .ToListAsync();

                var promotionDtos = promotions.Select(p => new PromotionDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    DiscountType = p.DiscountType,
                    DiscountValue = p.DiscountValue,
                    MinOrderValue = p.MinOrderAmount,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    UsageLimit = p.Quota,
                    UsedCount = p.UsedCount,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    TargetAudience = p.ApplicableDealers,
                    ApplicableVehicles = p.ApplicableModels,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return ServiceResponse<List<PromotionDto>>.SuccessResponse(promotionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting most used promotions");
                return ServiceResponse<List<PromotionDto>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }
    }
}