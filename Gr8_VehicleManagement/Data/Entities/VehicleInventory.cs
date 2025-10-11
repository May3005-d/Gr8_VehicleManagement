using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class VehicleInventory
    {
        public Guid Id { get; set; }
        public string VIN { get; set; } = string.Empty; // Vehicle Identification Number
        
        public Guid VersionId { get; set; }
        public VehicleVersion Version { get; set; } = null!;
        
        public Guid? DealerId { get; set; } // NULL = Factory warehouse
        public Dealer? Dealer { get; set; }
        
        public VehicleStatus Status { get; set; }
        
        public string? Location { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime? ImportedDate { get; set; }
        public DateTime? ReservedAt { get; set; }
        public DateTime? SoldAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

