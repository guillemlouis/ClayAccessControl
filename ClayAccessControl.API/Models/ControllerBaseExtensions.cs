using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;

namespace ClayAccessControl.API.Models
{
    public static class ControllerBaseExtensions
    {
        public static ApiResult<T> ApiOk<T>(this ControllerBase controller, T result, string message = null)
        {
            return new ApiResult<T>(result, message, 200);
        }

        public static ApiResult<T> ApiCreated<T>(this ControllerBase controller, T result, string message = null)
        {
            return new ApiResult<T>(result, message, 201);
        }

        public static ApiResult<object> ApiNoContent(this ControllerBase controller, string message = null)
        {
            return new ApiResult<object>(null, message, 204);
        }
    }
}