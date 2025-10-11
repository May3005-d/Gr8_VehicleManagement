using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateLastLoginAsync(Guid userId);
        Task<bool> ValidateUserAsync(string emailOrPhone, string password);
    }
}

