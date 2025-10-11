using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        
        // Customer Info
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        // Vehicle Info
        public Guid VersionId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleColor { get; set; } = string.Empty;
        
        // Dealer Info
        public Guid DealerId { get; set; }
        public string DealerName { get; set; } = string.Empty;
        
        // Pricing
        public decimal BasePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        
        // Status
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusDisplay => PaymentStatus.ToString();
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodDisplay => PaymentMethod.ToString();
        
        // Dates
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        
        // Contract
        public string? ContractNumber { get; set; }
        public DateTime? ContractSignedAt { get; set; }
        
        // Installment
        public string? InstallmentProvider { get; set; }
        public int? InstallmentTerm { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public decimal? InterestRate { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class OrderDetailDto : OrderDto
    {
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public string? VehicleSpecs { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? CancellationReason { get; set; }
        public string? Notes { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        // Related Data
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public List<FeedbackDto> Feedbacks { get; set; } = new List<FeedbackDto>();
    }

    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public Guid VersionId { get; set; }
        public Guid DealerId { get; set; }
        public Guid CreatedBy { get; set; }
        
        // Pricing
        public decimal BasePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        
        // Payment
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        
        // Installment
        public string? InstallmentProvider { get; set; }
        public int? InstallmentTerm { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public decimal? InterestRate { get; set; }
        
        public string? Notes { get; set; }
    }

    public class UpdateOrderDto
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string? ContractNumber { get; set; }
        public DateTime? ContractSignedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? CancellationReason { get; set; }
        public string? Notes { get; set; }
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        
        // Order Info
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        
        // Customer Info
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        // Payment Details
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeDisplay => PaymentType.ToString();
        public PaymentMethodType PaymentMethodType { get; set; }
        public string PaymentMethodTypeDisplay => PaymentMethodType.ToString();
        public PaymentStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        
        // Transaction Info
        public string? TransactionId { get; set; }
        public string? PaymentProof { get; set; }
        
        // Installment Info
        public int? InstallmentNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? LateFee { get; set; }
        
        // Refund Info
        public string? RefundReason { get; set; }
        public DateTime? RefundApprovedAt { get; set; }
        
        // Processor
        public Guid? ProcessedBy { get; set; }
        public string? ProcessorName { get; set; }
        
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PaymentDetailDto : PaymentDto
    {
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public string? OrderDetails { get; set; }
        public string? VehicleInfo { get; set; }
        public decimal? TotalOrderAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
    }

    public class CreatePaymentDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentMethodType PaymentMethodType { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentProof { get; set; }
        public int? InstallmentNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? LateFee { get; set; }
        public string? RefundReason { get; set; }
        public Guid? ProcessedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePaymentDto
    {
        public Guid Id { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentProof { get; set; }
        public decimal? LateFee { get; set; }
        public string? RefundReason { get; set; }
        public DateTime? RefundApprovedAt { get; set; }
        public string? Notes { get; set; }
    }

}

