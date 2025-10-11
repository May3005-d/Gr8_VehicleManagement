using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using System.Text.Json;

namespace Gr8_VehicleManagement.Common.Helpers
{
    public static class SessionHelper
    {
        private const string UserSessionKey = "CurrentUser";
        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";
        private const string UserTypeKey = "UserType";
        private const string DealerIdKey = "DealerId";

        public static void SetUserSession(this ISession session, UserDto user)
        {
            session.SetString(UserSessionKey, JsonSerializer.Serialize(user));
            session.SetString(UserIdKey, user.Id.ToString());
            session.SetString(UserNameKey, user.FullName);
            session.SetString(UserTypeKey, ((int)user.UserType).ToString());
            
            // Only set DealerId if it exists
            if (user.DealerId.HasValue)
            {
                session.SetString(DealerIdKey, user.DealerId.Value.ToString());
            }
        }

        public static UserDto? GetUserSession(this ISession session)
        {
            var userJson = session.GetString(UserSessionKey);
            if (string.IsNullOrEmpty(userJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<UserDto>(userJson);
            }
            catch
            {
                return null;
            }
        }

        public static Guid? GetUserId(this ISession session)
        {
            var userIdString = session.GetString(UserIdKey);
            if (Guid.TryParse(userIdString, out var userId))
                return userId;
            return null;
        }

        public static string? GetUserName(this ISession session)
        {
            return session.GetString(UserNameKey);
        }

        public static UserType? GetUserType(this ISession session)
        {
            var userTypeString = session.GetString(UserTypeKey);
            if (int.TryParse(userTypeString, out var userTypeInt))
                return (UserType)userTypeInt;
            return null;
        }

        public static Guid? GetDealerId(this ISession session)
        {
            var dealerIdString = session.GetString(DealerIdKey);
            if (Guid.TryParse(dealerIdString, out var dealerId))
                return dealerId;
            return null;
        }

        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetUserId().HasValue;
        }

        public static bool IsAdmin(this ISession session)
        {
            return session.GetUserType() == UserType.Admin;
        }

        public static bool IsEVMStaff(this ISession session)
        {
            return session.GetUserType() == UserType.EVMStaff;
        }

        public static bool IsDealerManager(this ISession session)
        {
            return session.GetUserType() == UserType.DealerManager;
        }

        public static bool IsDealerStaff(this ISession session)
        {
            return session.GetUserType() == UserType.DealerStaff;
        }

        public static bool IsCustomer(this ISession session)
        {
            return session.GetUserType() == UserType.Customer;
        }

        public static void ClearUserSession(this ISession session)
        {
            // Clear specific session keys instead of clearing all session data
            session.Remove(UserSessionKey);
            session.Remove(UserIdKey);
            session.Remove(UserNameKey);
            session.Remove(UserTypeKey);
            session.Remove(DealerIdKey);
        }

        // Additional session methods for authentication pages
        public static void SetUserId(this ISession session, Guid userId)
        {
            session.SetString(UserIdKey, userId.ToString());
        }

        public static void SetUserType(this ISession session, UserType userType)
        {
            session.SetString(UserTypeKey, ((int)userType).ToString());
        }

        public static void SetUsername(this ISession session, string username)
        {
            session.SetString(UserNameKey, username);
        }
    }
}

