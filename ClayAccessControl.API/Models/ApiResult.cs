using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;

namespace ClayAccessControl.API.Models
{
    public class ApiResult<T> : IActionResult
    {
        private readonly ApiResponse<T> _response;
        private readonly int _statusCode;

        public ApiResult(T result, string message = null, int statusCode = 200)
        {
            _response = new ApiResponse<T>(result, message);
            _statusCode = statusCode;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_response)
            {
                StatusCode = _statusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}