using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class OrderListViewModel
    {
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        // Filters
        public string? SearchTerm { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? DealerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        
        // Filter options
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public List<DealerDto> Dealers { get; set; } = new List<DealerDto>();
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        
        // Permissions
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewAll { get; set; }
    }

    public class DealerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}

