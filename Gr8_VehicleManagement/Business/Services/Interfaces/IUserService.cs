using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid id);
        Task<ServiceResponse<UserDto>> GetUserByUsernameAsync(string username);
        Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto dto);
        Task<ServiceResponse<UserDto>> UpdateUserAsync(UpdateUserDto dto);
        Task<ServiceResponse<bool>> DeleteUserAsync(Guid id);
        Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto);
        Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ServiceResponse<bool>> ValidateUserAsync(string username, string password);
    }
}
