using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public CustomerDetailDto Customer { get; set; } = null!;
        
        // Permissions
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanCreateOrder { get; set; }
        
        // Statistics
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int TestDriveCount { get; set; }
        public int InteractionCount { get; set; }
    }
}

