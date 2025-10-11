using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class CustomerListViewModel
    {
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        // Filters
        public string? SearchTerm { get; set; }
        public CustomerType? CustomerType { get; set; }
        public string? City { get; set; }
        public Guid? AssignedUserId { get; set; }
        
        // Filter options
        public List<string> Cities { get; set; } = new List<string>();
        public List<UserDto> Salespeople { get; set; } = new List<UserDto>();
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        
        // Permissions
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewAll { get; set; }
    }
}

