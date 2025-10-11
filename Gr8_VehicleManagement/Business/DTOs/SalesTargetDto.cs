using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.DTOs
{
    public class SalesTargetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TargetFor TargetFor { get; set; }
        public string TargetForDisplay => TargetFor.ToString();
        public Guid? DealerId { get; set; }
        public string? DealerName { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public decimal TargetAmount { get; set; }
        public int TargetQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AchievementAmount { get; set; }
        public int AchievementQuantity { get; set; }
        public decimal AchievementPercentage => TargetAmount > 0 ? (AchievementAmount / TargetAmount) * 100 : 0;
        public decimal CommissionRate { get; set; }
        public decimal CommissionEarned { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateSalesTargetDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TargetFor TargetFor { get; set; }
        public Guid? DealerId { get; set; }
        public Guid? UserId { get; set; }
        public decimal TargetAmount { get; set; }
        public int TargetQuantity { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);
        public decimal CommissionRate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSalesTargetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TargetFor TargetFor { get; set; }
        public Guid? DealerId { get; set; }
        public Guid? UserId { get; set; }
        public decimal TargetAmount { get; set; }
        public int TargetQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal CommissionRate { get; set; }
        public bool IsActive { get; set; }
    }

    public class SalesTargetSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TargetForDisplay { get; set; } = string.Empty;
        public string? DealerName { get; set; }
        public string? UserName { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal AchievementAmount { get; set; }
        public decimal AchievementPercentage { get; set; }
        public decimal CommissionEarned { get; set; }
        public bool IsActive { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRemaining => (EndDate - DateTime.Today).Days;
    }
}
