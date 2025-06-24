using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bike.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            if (_roles.Length > 0)
            {
                var userRole = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!_roles.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }
        }
    }

    // Helper class để định nghĩa roles
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string User = "User";
        public const string AdminOrStaff = "Admin,Staff";
        public const string All = "Admin,Staff,User";
    }
}