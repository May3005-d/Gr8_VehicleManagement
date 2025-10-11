using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(ApplicationDbContext context, ILogger<VehicleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<VehicleModelDto>>> GetAllModelsAsync(
            string? searchTerm = null,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.VehicleModels
                    .Include(m => m.VehicleVersions)
                    .Include(m => m.VehicleImages)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(m => 
                        m.Name.Contains(searchTerm) || 
                        m.Code.Contains(searchTerm) ||
                        (m.Description != null && m.Description.Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(m => m.Category == category);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(m => m.IsActive == isActive.Value);
                }

                // Price filtering
                if (minPrice.HasValue || maxPrice.HasValue)
                {
                    query = query.Where(m => m.VehicleVersions.Any(v =>
                        (!minPrice.HasValue || v.SellingPrice >= minPrice.Value) &&
                        (!maxPrice.HasValue || v.SellingPrice <= maxPrice.Value)));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var models = await query
                    .OrderBy(m => m.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var modelDtos = models.Select(m => new VehicleModelDto
                {
                    Id = m.Id,
                    Code = m.Code,
                    Name = m.Name,
                    Category = m.Category,
                    Brand = m.Brand,
                    Description = m.Description,
                    EngineType = m.EngineType,
                    BatteryCapacity = m.BatteryCapacity,
                    Range = m.Range,
                    ChargingTime = m.ChargingTime,
                    SeatingCapacity = m.SeatingCapacity,
                    Features = m.Features,
                    IsActive = m.IsActive,
                    LaunchedAt = m.LaunchedAt,
                    VersionCount = m.VehicleVersions.Count,
                    InventoryCount = _context.VehicleInventories.Count(i => i.Version.ModelId == m.Id),
                    MinPrice = m.VehicleVersions.Any() ? m.VehicleVersions.Min(v => v.SellingPrice) : null,
                    MaxPrice = m.VehicleVersions.Any() ? m.VehicleVersions.Max(v => v.SellingPrice) : null,
                    Images = m.VehicleImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new VehicleImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            ImageType = i.ImageType,
                            DisplayOrder = i.DisplayOrder,
                            IsDefault = i.IsDefault
                        }).ToList()
                }).ToList();

                return ServiceResponse<List<VehicleModelDto>>.SuccessResponse(modelDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle models");
                return ServiceResponse<List<VehicleModelDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách xe");
            }
        }

        public async Task<ServiceResponse<VehicleModelDto>> GetModelByIdAsync(Guid id)
        {
            try
            {
                var model = await _context.VehicleModels
                    .Include(m => m.VehicleVersions)
                    .Include(m => m.VehicleImages)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (model == null)
                {
                    return ServiceResponse<VehicleModelDto>.ErrorResponse("Không tìm thấy mẫu xe");
                }

                var dto = new VehicleModelDto
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    Category = model.Category,
                    Brand = model.Brand,
                    Description = model.Description,
                    EngineType = model.EngineType,
                    BatteryCapacity = model.BatteryCapacity,
                    Range = model.Range,
                    ChargingTime = model.ChargingTime,
                    SeatingCapacity = model.SeatingCapacity,
                    Features = model.Features,
                    IsActive = model.IsActive,
                    LaunchedAt = model.LaunchedAt,
                    VersionCount = model.VehicleVersions.Count,
                    InventoryCount = await _context.VehicleInventories.CountAsync(i => i.Version.ModelId == model.Id),
                    MinPrice = model.VehicleVersions.Any() ? model.VehicleVersions.Min(v => v.SellingPrice) : null,
                    MaxPrice = model.VehicleVersions.Any() ? model.VehicleVersions.Max(v => v.SellingPrice) : null,
                    Images = model.VehicleImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new VehicleImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            ImageType = i.ImageType,
                            DisplayOrder = i.DisplayOrder,
                            IsDefault = i.IsDefault
                        }).ToList()
                };

                return ServiceResponse<VehicleModelDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle model by ID");
                return ServiceResponse<VehicleModelDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<VehicleModelDto>> GetVehicleModelByIdAsync(Guid id)
        {
            try
            {
                var model = await _context.VehicleModels
                    .Include(m => m.VehicleVersions)
                    .Include(m => m.VehicleImages)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (model == null)
                {
                    return ServiceResponse<VehicleModelDto>.ErrorResponse("Không tìm thấy mẫu xe");
                }

                var dto = new VehicleModelDto
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    Category = model.Category,
                    Brand = model.Brand,
                    Description = model.Description,
                    EngineType = model.EngineType,
                    BatteryCapacity = model.BatteryCapacity,
                    Range = model.Range,
                    ChargingTime = model.ChargingTime,
                    SeatingCapacity = model.SeatingCapacity,
                    Features = model.Features,
                    IsActive = model.IsActive,
                    LaunchedAt = model.LaunchedAt,
                    VersionCount = model.VehicleVersions.Count,
                    InventoryCount = await _context.VehicleInventories.CountAsync(i => i.Version.ModelId == model.Id),
                    MinPrice = model.VehicleVersions.Any() ? model.VehicleVersions.Min(v => v.SellingPrice) : null,
                    MaxPrice = model.VehicleVersions.Any() ? model.VehicleVersions.Max(v => v.SellingPrice) : null,
                    Images = model.VehicleImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new VehicleImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            ImageType = i.ImageType,
                            DisplayOrder = i.DisplayOrder,
                            IsDefault = i.IsDefault
                        }).ToList()
                };

                return ServiceResponse<VehicleModelDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle model by ID");
                return ServiceResponse<VehicleModelDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<List<string>>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.VehicleModels
                    .Select(m => m.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                return ServiceResponse<List<string>>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return ServiceResponse<List<string>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<List<VehicleVersionDto>>> GetVersionsByModelIdAsync(Guid modelId)
        {
            try
            {
                var versions = await _context.VehicleVersions
                    .Include(v => v.Model)
                    .Include(v => v.VehicleImages)
                    .Where(v => v.ModelId == modelId)
                    .ToListAsync();

                var versionDtos = versions.Select(v => new VehicleVersionDto
                {
                    Id = v.Id,
                    ModelId = v.ModelId,
                    ModelName = v.Model.Name,
                    VersionName = v.VersionName,
                    ColorName = v.ColorName,
                    ColorCode = v.ColorCode,
                    BatteryCapacity = v.BatteryCapacity,
                    Range = v.Range,
                    MaxSpeed = v.MaxSpeed,
                    ChargingTime = v.ChargingTime,
                    SeatingCapacity = v.SeatingCapacity,
                    Features = v.Features,
                    BasePrice = v.BasePrice,
                    SellingPrice = v.SellingPrice,
                    IsActive = v.IsActive,
                    AvailableStock = _context.VehicleInventories.Count(i => 
                        i.VersionId == v.Id && 
                        i.Status == VehicleStatus.Available),
                    Images = v.VehicleImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new VehicleImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            ImageType = i.ImageType,
                            DisplayOrder = i.DisplayOrder,
                            IsDefault = i.IsDefault
                        }).ToList()
                }).ToList();

                return ServiceResponse<List<VehicleVersionDto>>.SuccessResponse(versionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting versions");
                return ServiceResponse<List<VehicleVersionDto>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<VehicleVersionDto>> GetVersionByIdAsync(Guid id)
        {
            try
            {
                var version = await _context.VehicleVersions
                    .Include(v => v.Model)
                    .Include(v => v.VehicleImages)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (version == null)
                {
                    return ServiceResponse<VehicleVersionDto>.ErrorResponse("Không tìm thấy phiên bản");
                }

                var dto = new VehicleVersionDto
                {
                    Id = version.Id,
                    ModelId = version.ModelId,
                    ModelName = version.Model.Name,
                    VersionName = version.VersionName,
                    ColorName = version.ColorName,
                    ColorCode = version.ColorCode,
                    BatteryCapacity = version.BatteryCapacity,
                    Range = version.Range,
                    MaxSpeed = version.MaxSpeed,
                    ChargingTime = version.ChargingTime,
                    SeatingCapacity = version.SeatingCapacity,
                    Features = version.Features,
                    BasePrice = version.BasePrice,
                    SellingPrice = version.SellingPrice,
                    IsActive = version.IsActive,
                    AvailableStock = await _context.VehicleInventories
                        .CountAsync(i => i.VersionId == version.Id && i.Status == VehicleStatus.Available),
                    Images = version.VehicleImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new VehicleImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            ImageType = i.ImageType,
                            DisplayOrder = i.DisplayOrder,
                            IsDefault = i.IsDefault
                        }).ToList()
                };

                return ServiceResponse<VehicleVersionDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version by ID");
                return ServiceResponse<VehicleVersionDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetAvailableStockAsync(Guid versionId, Guid? dealerId = null)
        {
            try
            {
                var query = _context.VehicleInventories
                    .Where(i => i.VersionId == versionId && i.Status == VehicleStatus.Available);

                if (dealerId.HasValue)
                {
                    query = query.Where(i => i.DealerId == dealerId.Value);
                }

                var count = await query.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available stock");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalModelsCountAsync()
        {
            try
            {
                var count = await _context.VehicleModels.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total models count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalInventoryCountAsync()
        {
            try
            {
                var count = await _context.VehicleInventories.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total inventory count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<VehicleModelDto>> CreateVehicleModelAsync(CreateVehicleModelDto dto)
        {
            try
            {
                // Check if code already exists
                var existingModel = await _context.VehicleModels
                    .FirstOrDefaultAsync(m => m.Code == dto.Code);
                
                if (existingModel != null)
                {
                    return ServiceResponse<VehicleModelDto>.ErrorResponse("Mã mẫu xe đã tồn tại");
                }

                // Create new vehicle model
                var model = new Data.Entities.VehicleModel
                {
                    Id = Guid.NewGuid(),
                    Code = dto.Code,
                    Name = dto.Name,
                    Category = dto.Category,
                    Brand = dto.Brand,
                    Description = dto.Description,
                    EngineType = dto.EngineType,
                    BatteryCapacity = dto.BatteryCapacity,
                    Range = dto.Range,
                    ChargingTime = dto.ChargingTime,
                    SeatingCapacity = dto.SeatingCapacity,
                    Features = dto.Features,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.VehicleModels.Add(model);

                // Add images if provided
                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    for (int i = 0; i < dto.ImageUrls.Count; i++)
                    {
                        var image = new Data.Entities.VehicleImage
                        {
                            Id = Guid.NewGuid(),
                            VehicleModelId = model.Id,
                            ImageUrl = dto.ImageUrls[i],
                            ImageType = "Model",
                            DisplayOrder = i + 1,
                            IsDefault = i == 0
                        };
                        _context.VehicleImages.Add(image);
                    }
                }

                await _context.SaveChangesAsync();

                // Return the created model
                var result = await GetVehicleModelByIdAsync(model.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle model");
                return ServiceResponse<VehicleModelDto>.ErrorResponse("Đã xảy ra lỗi khi tạo mẫu xe");
            }
        }

        public async Task<ServiceResponse<VehicleModelDto>> UpdateVehicleModelAsync(UpdateVehicleModelDto dto)
        {
            try
            {
                var model = await _context.VehicleModels
                    .Include(m => m.VehicleImages)
                    .FirstOrDefaultAsync(m => m.Id == dto.Id);

                if (model == null)
                {
                    return ServiceResponse<VehicleModelDto>.ErrorResponse("Không tìm thấy mẫu xe");
                }

                // Check if code already exists (excluding current model)
                var existingModel = await _context.VehicleModels
                    .FirstOrDefaultAsync(m => m.Code == dto.Code && m.Id != dto.Id);
                
                if (existingModel != null)
                {
                    return ServiceResponse<VehicleModelDto>.ErrorResponse("Mã mẫu xe đã tồn tại");
                }

                // Update model properties
                model.Code = dto.Code;
                model.Name = dto.Name;
                model.Category = dto.Category;
                model.Brand = dto.Brand;
                model.Description = dto.Description;
                model.EngineType = dto.EngineType;
                model.BatteryCapacity = dto.BatteryCapacity;
                model.Range = dto.Range;
                model.ChargingTime = dto.ChargingTime;
                model.SeatingCapacity = dto.SeatingCapacity;
                model.Features = dto.Features;
                model.IsActive = dto.IsActive;
                model.UpdatedAt = DateTime.UtcNow;

                // Add new images if provided
                if (dto.NewImageUrls != null && dto.NewImageUrls.Any())
                {
                    var maxOrder = model.VehicleImages.Any() ? model.VehicleImages.Max(i => i.DisplayOrder) : 0;
                    
                    for (int i = 0; i < dto.NewImageUrls.Count; i++)
                    {
                        var image = new Data.Entities.VehicleImage
                        {
                            Id = Guid.NewGuid(),
                            VehicleModelId = model.Id,
                            ImageUrl = dto.NewImageUrls[i],
                            ImageType = "Model",
                            DisplayOrder = maxOrder + i + 1,
                            IsDefault = false
                        };
                        _context.VehicleImages.Add(image);
                    }
                }

                await _context.SaveChangesAsync();

                // Return the updated model
                var result = await GetVehicleModelByIdAsync(model.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle model");
                return ServiceResponse<VehicleModelDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật mẫu xe");
            }
        }

        // Placeholder implementations for CRUD operations
        public Task<ServiceResponse<VehicleModelDto>> CreateModelAsync(VehicleModelDto dto)
        {
            throw new NotImplementedException("Will implement in Create page");
        }

        public Task<ServiceResponse<VehicleModelDto>> UpdateModelAsync(VehicleModelDto dto)
        {
            throw new NotImplementedException("Will implement in Edit page");
        }

        public Task<ServiceResponse<bool>> DeleteModelAsync(Guid id)
        {
            throw new NotImplementedException("Will implement in Delete functionality");
        }

        public Task<ServiceResponse<VehicleVersionDto>> CreateVersionAsync(VehicleVersionDto dto)
        {
            throw new NotImplementedException("Will implement in Create page");
        }

        public Task<ServiceResponse<VehicleVersionDto>> UpdateVersionAsync(VehicleVersionDto dto)
        {
            throw new NotImplementedException("Will implement in Edit page");
        }

        public Task<ServiceResponse<bool>> DeleteVersionAsync(Guid id)
        {
            throw new NotImplementedException("Will implement in Delete functionality");
        }

        public Task<ServiceResponse<List<VehicleInventoryDto>>> GetInventoryAsync(Guid? modelId = null, Guid? dealerId = null, string? status = null)
        {
            throw new NotImplementedException("Will implement when needed");
        }
    }
}

