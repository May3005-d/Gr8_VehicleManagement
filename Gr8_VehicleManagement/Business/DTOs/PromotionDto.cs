using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.DTOs
{
    public class PromotionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? TargetAudience { get; set; }
        public string? ApplicableVehicles { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Computed properties
        public string DiscountDisplay => DiscountType == DiscountType.Percentage 
            ? $"{DiscountValue}%" 
            : $"{DiscountValue:N0} VNĐ";
        
        public bool IsValid => IsActive && 
                               DateTime.Now >= StartDate && 
                               DateTime.Now <= EndDate &&
                               (UsageLimit == null || UsedCount < UsageLimit);
        
        public int RemainingUses => UsageLimit.HasValue ? UsageLimit.Value - UsedCount : int.MaxValue;
    }

    public class CreatePromotionDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? TargetAudience { get; set; }
        public string? ApplicableVehicles { get; set; }
    }

    public class UpdatePromotionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? TargetAudience { get; set; }
        public string? ApplicableVehicles { get; set; }
    }

    public class ApplyPromotionDto
    {
        public Guid OrderId { get; set; }
        public string PromotionCode { get; set; } = string.Empty;
        public decimal OrderValue { get; set; }
        public List<Guid> VehicleIds { get; set; } = new List<Guid>();
        public Guid? CustomerId { get; set; }
    }

    public class PromotionApplicationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public PromotionDto? AppliedPromotion { get; set; }
    }
}
