using AutoMapper;
using Gr8_VehicleManagement.Data.Entities;
using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Basic Entity Mappings (Simple 1:1 mapping)
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<VehicleModel, VehicleModelDto>().ReverseMap();
            CreateMap<VehicleVersion, VehicleVersionDto>().ReverseMap();
            
            // Payment DTOs
            CreateMap<Payment, CreatePaymentDto>().ReverseMap();
            CreateMap<Payment, UpdatePaymentDto>().ReverseMap();
            CreateMap<Payment, PaymentDetailDto>().ReverseMap();
            
            // Customer DTOs
            CreateMap<Customer, CreateCustomerDto>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();
            
            // Order DTOs
            CreateMap<Order, CreateOrderDto>().ReverseMap();
            CreateMap<Order, UpdateOrderDto>().ReverseMap();
        }
    }
}