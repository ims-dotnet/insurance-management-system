using System.Net;
using System.Text.Json;
using FluentValidation;
using InsureTrust.PaymentService.Wrappers;

namespace InsureTrust.PaymentService.Exceptions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred in PaymentService.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is ValidationException validationEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                var validationResponse = ApiResponse<object>.FailureResponse("Validation failed.", errors);

                await context.Response.WriteAsync(JsonSerializer.Serialize(validationResponse));
                return;
            }

            int statusCode = exception switch
            {
                BadRequestException => (int)HttpStatusCode.BadRequest,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = ApiResponse<object>.FailureResponse(exception.Message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}