using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class PaymentListViewModel
    {
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        // Filters
        public string? SearchTerm { get; set; }
        public PaymentStatus? Status { get; set; }
        public PaymentType? PaymentType { get; set; }
        public PaymentMethodType? PaymentMethodType { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        
        // Filter options
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        
        // Permissions
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewAll { get; set; }
    }

    public class PaymentDetailsViewModel
    {
        public PaymentDetailDto Payment { get; set; } = null!;
        
        // Permissions
        public bool CanEdit { get; set; }
        public bool CanRefund { get; set; }
        public bool CanApprove { get; set; }
        
        // Statistics
        public decimal TotalPaidForOrder { get; set; }
        public decimal RemainingOrderAmount { get; set; }
        public int PaymentCountForOrder { get; set; }
    }
}
