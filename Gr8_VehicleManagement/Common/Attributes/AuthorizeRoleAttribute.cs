using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gr8_VehicleManagement.Common.Attributes
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly UserType[] _allowedUserTypes;

        public AuthorizeRoleAttribute(params UserType[] allowedUserTypes)
        {
            _allowedUserTypes = allowedUserTypes;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            
            if (!session.IsAuthenticated())
            {
                context.Result = new RedirectToPageResult("/Account/Login", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            var userType = session.GetUserType();
            if (userType == null || !_allowedUserTypes.Contains(userType.Value))
            {
                context.Result = new RedirectToPageResult("/Error", new { message = "Bạn không có quyền truy cập trang này" });
            }

            base.OnActionExecuting(context);
        }
    }
}

