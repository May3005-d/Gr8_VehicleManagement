namespace Gr8_VehicleManagement.Data.Entities
{
    public class VehicleModel
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
        public string? Specifications { get; set; } // JSON
        public bool IsActive { get; set; }
        public DateTime? LaunchedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<VehicleVersion> VehicleVersions { get; set; } = new List<VehicleVersion>();
        public ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
    }
}

