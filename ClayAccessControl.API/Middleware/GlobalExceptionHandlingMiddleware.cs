using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ClayAccessControl.Core.Exceptions;
using ClayAccessControl.API.Models;

namespace ClayAccessControl.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case UnauthorizedException:
                    status = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case ConflictException:
                    status = HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;
                case ForbiddenException:
                    status = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(exception, "An error occurred: {Message}", message);

            var response = new ApiResponse<object>(null, message, false);
            var result = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(result);
        }
    }
}