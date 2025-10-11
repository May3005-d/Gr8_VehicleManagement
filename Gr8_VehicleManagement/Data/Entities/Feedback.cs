using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Data.Entities
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public string FeedbackNumber { get; set; } = string.Empty;
        
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }
        
        public Guid DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;
        
        public FeedbackType Type { get; set; }
        public FeedbackType FeedbackType { get; set; } // Added for compatibility
        
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty; // Changed from Subject
        public string Description { get; set; } = string.Empty; // Changed from Content
        public string? Notes { get; set; } // Added for internal notes
        public string? Evidence { get; set; } // JSON: array of URLs
        public int? Rating { get; set; }
        
        public FeedbackStatus Status { get; set; }
        public Priority Priority { get; set; }
        
        public Guid? AssignedUserId { get; set; } // Changed from AssignedTo
        public User? AssignedUser { get; set; }
        
        public DateTime? SLA { get; set; }
        public string? Resolution { get; set; }
        public decimal? Compensation { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } // Added
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}

