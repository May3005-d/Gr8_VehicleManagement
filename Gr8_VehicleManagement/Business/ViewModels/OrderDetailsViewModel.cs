using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderDetailDto Order { get; set; } = null!;
        
        // Permissions
        public bool CanEdit { get; set; }
        public bool CanCancel { get; set; }
        public bool CanDeliver { get; set; }
        public bool CanCreatePayment { get; set; }
        
        // Statistics
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public int PaymentCount { get; set; }
        public int FeedbackCount { get; set; }
    }
}

