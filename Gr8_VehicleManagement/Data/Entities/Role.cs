namespace Gr8_VehicleManagement.Data.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

