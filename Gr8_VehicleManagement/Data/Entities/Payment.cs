using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        public decimal Amount { get; set; }
        
        public PaymentType PaymentType { get; set; }
        public PaymentMethodType PaymentMethodType { get; set; }
        
        public DateTime PaymentDate { get; set; }
        public Enums.PaymentStatus Status { get; set; }
        
        public string? TransactionId { get; set; }
        public string? PaymentProof { get; set; } // Image URL
        
        // Refund info (merged)
        public string? RefundReason { get; set; }
        public Guid? RefundApprovedBy { get; set; }
        public DateTime? RefundApprovedAt { get; set; }
        
        // Installment schedule (merged)
        public int? InstallmentNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? LateFee { get; set; }
        
        public Guid? ProcessedBy { get; set; }
        public User? Processor { get; set; }
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

