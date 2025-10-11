namespace Gr8_VehicleManagement.Data.Entities
{
    public class VehicleVersion
    {
        public Guid Id { get; set; }
        public Guid ModelId { get; set; }
        public VehicleModel Model { get; set; } = null!;
        
        public string VersionName { get; set; } = string.Empty;
        
        // Color
        public string ColorName { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        
        // Specifications
        public int? BatteryCapacity { get; set; } // kWh
        public int? Range { get; set; } // km
        public int? MaxSpeed { get; set; } // km/h
        public int? ChargingTime { get; set; } // minutes
        public int? SeatingCapacity { get; set; }
        public string? Features { get; set; } // JSON
        
        // Pricing
        public decimal BasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<VehicleInventory> VehicleInventories { get; set; } = new List<VehicleInventory>();
        public ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

