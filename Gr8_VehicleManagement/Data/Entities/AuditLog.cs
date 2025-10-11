namespace Gr8_VehicleManagement.Data.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        
        public string? OldValue { get; set; } // JSON
        public string? NewValue { get; set; } // JSON
        
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        
        public string Result { get; set; } = string.Empty; // Success, Failed
        public string? ErrorMessage { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}

