using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class SalesTarget
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TargetFor TargetFor { get; set; }
        
        public Guid? DealerId { get; set; }
        public Dealer? Dealer { get; set; }
        
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        
        public decimal TargetAmount { get; set; }
        public int TargetQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public decimal AchievementAmount { get; set; }
        public int AchievementQuantity { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal CommissionEarned { get; set; }
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

