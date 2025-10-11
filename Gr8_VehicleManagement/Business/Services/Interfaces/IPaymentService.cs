using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IPaymentService
    {
        // List & Search
        Task<ServiceResponse<List<PaymentDto>>> GetAllPaymentsAsync(
            string? searchTerm = null,
            PaymentStatus? status = null,
            PaymentType? paymentType = null,
            PaymentMethodType? paymentMethodType = null,
            Guid? orderId = null,
            Guid? customerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10);
        
        Task<ServiceResponse<int>> GetTotalPaymentsCountAsync();
        
        // CRUD
        Task<ServiceResponse<PaymentDetailDto>> GetPaymentByIdAsync(Guid id);
        Task<ServiceResponse<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto);
        Task<ServiceResponse<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto);
        Task<ServiceResponse<bool>> DeletePaymentAsync(Guid id);
        
        // Status Updates
        Task<ServiceResponse<bool>> UpdatePaymentStatusAsync(Guid id, PaymentStatus status);
        Task<ServiceResponse<bool>> ApproveRefundAsync(Guid id, string reason);
        Task<ServiceResponse<bool>> ProcessPaymentAsync(Guid id, string transactionId);
        
        // Statistics
        Task<ServiceResponse<decimal>> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ServiceResponse<int>> GetPaymentsCountByStatusAsync(PaymentStatus status);
        Task<ServiceResponse<decimal>> GetPaymentsByOrderAsync(Guid orderId);
        Task<ServiceResponse<List<PaymentDto>>> GetPaymentsByOrderAsync(Guid orderId, int pageNumber = 1, int pageSize = 10);
    }
}
