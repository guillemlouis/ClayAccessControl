using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;

namespace ClayAccessControl.API.Models
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }

        public ApiResponse(T result, string message = null, bool success = true)
        {
            Result = result;
            Message = message;
            Success = success;
        }
    }
}