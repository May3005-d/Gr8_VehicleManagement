using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class Dealer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DealerStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<VehicleInventory> VehicleInventories { get; set; } = new List<VehicleInventory>();
        public ICollection<SalesTarget> SalesTargets { get; set; } = new List<SalesTarget>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}

