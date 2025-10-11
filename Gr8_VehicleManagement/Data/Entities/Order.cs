using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty; // Added for compatibility
        
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        public Guid DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;
        
        public Guid CreatedBy { get; set; }
        public User Creator { get; set; } = null!;
        
        // Vehicle
        public Guid VersionId { get; set; }
        public VehicleVersion VehicleVersion { get; set; } = null!;
        
        public Guid? VehicleInventoryId { get; set; }
        public VehicleInventory? VehicleInventory { get; set; }
        
        // Pricing
        public decimal BasePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        
        // Dates
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        
        // Status
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        
        // Contract info (merged)
        public string? ContractNumber { get; set; }
        public DateTime? ContractSignedAt { get; set; }
        public string? CustomerSignature { get; set; } // URL or base64
        
        // Delivery info (merged)
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? DeliveryPhotos { get; set; } // JSON: array of URLs
        
        // Installment info (merged)
        public string? InstallmentProvider { get; set; }
        public int? InstallmentTerm { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public decimal? InterestRate { get; set; }
        
        // Notes
        public string? CancellationReason { get; set; }
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}

