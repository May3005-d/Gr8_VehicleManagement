using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        
        // Address
        public string? Address { get; set; }
        public string? City { get; set; }
        
        // Customer Type
        public CustomerType CustomerType { get; set; }
        
        // CRM
        public string? Source { get; set; }
        
        public Guid? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

