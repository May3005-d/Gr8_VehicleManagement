using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string FullName { get; set; } = string.Empty;
        public UserType UserType { get; set; } = UserType.Customer;
        public Guid? DealerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public Dealer? Dealer { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Customer> AssignedCustomers { get; set; } = new List<Customer>();
        public ICollection<Order> CreatedOrders { get; set; } = new List<Order>();
        public ICollection<SalesTarget> SalesTargets { get; set; } = new List<SalesTarget>();
        public ICollection<Feedback> AssignedFeedbacks { get; set; } = new List<Feedback>();
        public ICollection<Payment> ProcessedPayments { get; set; } = new List<Payment>();
    }
}

