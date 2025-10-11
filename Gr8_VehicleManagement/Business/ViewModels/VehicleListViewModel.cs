using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class VehicleListViewModel
    {
        public List<VehicleModelDto> Vehicles { get; set; } = new List<VehicleModelDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        // Filters
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }
        
        // Categories for filter
        public List<string> Categories { get; set; } = new List<string>();
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

