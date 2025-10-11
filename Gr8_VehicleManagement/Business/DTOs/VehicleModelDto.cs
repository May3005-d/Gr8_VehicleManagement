namespace Gr8_VehicleManagement.Business.DTOs
{
    public class VehicleModelDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EngineType { get; set; }
        public decimal? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public decimal? ChargingTime { get; set; }
        public int? SeatingCapacity { get; set; }
        public string? Features { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LaunchedAt { get; set; }
        public int VersionCount { get; set; }
        public int InventoryCount { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<VehicleImageDto> Images { get; set; } = new List<VehicleImageDto>();
    }

    public class VehicleVersionDto
    {
        public Guid Id { get; set; }
        public Guid ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string VersionName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        
        // Specs
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public int? MaxSpeed { get; set; }
        public int? ChargingTime { get; set; }
        public int? SeatingCapacity { get; set; }
        public string? Features { get; set; }
        
        // Pricing
        public decimal BasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        
        public bool IsActive { get; set; }
        public int AvailableStock { get; set; }
        public List<VehicleImageDto> Images { get; set; } = new List<VehicleImageDto>();
    }

    public class VehicleImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
    }

    public class VehicleInventoryDto
    {
        public Guid Id { get; set; }
        public string VIN { get; set; } = string.Empty;
        public Guid VersionId { get; set; }
        public string VersionName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public Guid? DealerId { get; set; }
        public string? DealerName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime? ImportedDate { get; set; }
    }

    public class CreateVehicleModelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EngineType { get; set; }
        public decimal? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public decimal? ChargingTime { get; set; }
        public int? SeatingCapacity { get; set; }
        public string? Features { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string>? ImageUrls { get; set; }
        public IFormFileCollection? Images { get; set; }
    }

    public class UpdateVehicleModelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EngineType { get; set; }
        public decimal? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public decimal? ChargingTime { get; set; }
        public int? SeatingCapacity { get; set; }
        public string? Features { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string>? NewImageUrls { get; set; }
        public IFormFileCollection? Images { get; set; }
    }
}

