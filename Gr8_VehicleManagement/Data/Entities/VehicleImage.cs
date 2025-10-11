namespace Gr8_VehicleManagement.Data.Entities
{
    public class VehicleImage
    {
        public Guid Id { get; set; }
        
        public Guid? VehicleModelId { get; set; }
        public VehicleModel? VehicleModel { get; set; }
        
        public Guid? VehicleVersionId { get; set; }
        public VehicleVersion? VehicleVersion { get; set; }
        
        public Guid? VehicleInventoryId { get; set; }
        public VehicleInventory? VehicleInventory { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty; // Model, Version, Actual, Interior, Exterior, 360View
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
        
        public DateTime UploadedAt { get; set; }
    }
}

