using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ClayAccessControl.API.Filters{
    public class UserIdFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userId, out int parsedUserId))
                {
                    context.HttpContext.Items["UserId"] = parsedUserId;
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
                base.OnActionExecuting(context);
            }
        }
}
