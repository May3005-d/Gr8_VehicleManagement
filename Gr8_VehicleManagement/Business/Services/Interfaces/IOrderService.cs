    using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IOrderService
    {
        // List & Search
        Task<ServiceResponse<List<OrderDto>>> GetAllOrdersAsync(
            string? searchTerm = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            Guid? customerId = null,
            Guid? dealerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10);
        
        Task<ServiceResponse<int>> GetTotalOrdersCountAsync();
        
        // CRUD
        Task<ServiceResponse<OrderDetailDto>> GetOrderByIdAsync(Guid id);
        Task<ServiceResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto);
        Task<ServiceResponse<OrderDto>> UpdateOrderAsync(UpdateOrderDto dto);
        Task<ServiceResponse<bool>> CancelOrderAsync(Guid id, string reason);
        
        // Status Updates
        Task<ServiceResponse<bool>> UpdateOrderStatusAsync(Guid id, OrderStatus status);
        Task<ServiceResponse<bool>> UpdatePaymentStatusAsync(Guid id, PaymentStatus status);
        Task<ServiceResponse<bool>> MarkAsDeliveredAsync(Guid id, string? deliveryNotes = null);
        
        // Statistics
        Task<ServiceResponse<decimal>> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ServiceResponse<int>> GetOrdersCountByStatusAsync(OrderStatus status);
        Task<ServiceResponse<decimal>> GetRemainingAmountAsync(Guid orderId);
    }
}

