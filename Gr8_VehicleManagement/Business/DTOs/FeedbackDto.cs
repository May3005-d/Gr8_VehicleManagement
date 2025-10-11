using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.DTOs
{
    public class FeedbackDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeedbackType FeedbackType { get; set; }
        public string FeedbackTypeDisplay => FeedbackType.ToString();
        public FeedbackStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public Priority Priority { get; set; }
        public string PriorityDisplay => Priority.ToString();
        
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        
        public Guid? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        
        public Guid? OrderId { get; set; }
        public string? OrderCode { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        
        public string? Resolution { get; set; }
        public string? Notes { get; set; }
        
        public int DaysOpen => (DateTime.Now - CreatedAt).Days;
        public bool IsOverdue => Status != FeedbackStatus.Resolved && DaysOpen > 7;
    }

    public class CreateFeedbackDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeedbackType FeedbackType { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
        
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        
        public Guid? OrderId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateFeedbackDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeedbackType FeedbackType { get; set; }
        public Priority Priority { get; set; }
        public FeedbackStatus Status { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? Resolution { get; set; }
        public string? Notes { get; set; }
    }

    public class FeedbackSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public FeedbackType FeedbackType { get; set; }
        public FeedbackStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string? CustomerName { get; set; }
        public string? AssignedUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DaysOpen { get; set; }
        public bool IsOverdue { get; set; }
    }
}
