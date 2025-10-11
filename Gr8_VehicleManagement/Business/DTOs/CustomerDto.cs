using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gr8_VehicleManagement.Business.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? IDNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        public string CustomerTypeDisplay => CustomerType.ToString();
        
        // CRM Data
        public string? Source { get; set; }
        public DateTime? FirstContactDate { get; set; }
        public string? AssignedSalesperson { get; set; }
        public Guid? AssignedUserId { get; set; }
        
        // Statistics
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int TestDriveCount { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CustomerDetailDto : CustomerDto
    {
        public List<CustomerInteractionDto> RecentInteractions { get; set; } = new List<CustomerInteractionDto>();
        public List<TestDriveDto> TestDrives { get; set; } = new List<TestDriveDto>();
        public List<CustomerOrderDto> Orders { get; set; } = new List<CustomerOrderDto>();
    }

    public class CustomerInteractionDto
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? ContactedBy { get; set; }
    }

    public class TestDriveDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class CustomerOrderDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(256, ErrorMessage = "Email không được vượt quá 256 ký tự")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Thành phố không được vượt quá 100 ký tự")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Loại khách hàng là bắt buộc")]
        public CustomerType CustomerType { get; set; }

        [StringLength(50, ErrorMessage = "Nguồn khách hàng không được vượt quá 50 ký tự")]
        public string? Source { get; set; }

        public Guid? AssignedUserId { get; set; }
    }

    public class UpdateCustomerDto : CreateCustomerDto
    {
        public Guid Id { get; set; }
    }
}

