using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class Promotion
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        
        // Applicable conditions (JSON for flexibility)
        public string? ApplicableModels { get; set; } // JSON: array of ModelIds
        public string? ApplicableDealers { get; set; } // JSON: array of DealerIds
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        
        // Validity
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Quota { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        
        // Rules
        public int Priority { get; set; }
        public bool CanCombineWithOthers { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

