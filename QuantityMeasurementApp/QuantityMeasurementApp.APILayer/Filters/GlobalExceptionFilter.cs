using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuantityMeasurementApp.APILayer.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception,
                "Unhandled exception occurred while executing request.");

            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "error occurred.";

            if (context.Exception is ArgumentException ||
                context.Exception is InvalidOperationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = context.Exception.Message;
            }
            else if (context.Exception is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = context.Exception.Message;
            }

            context.Result = new ObjectResult(new
            {
                StatusCode = statusCode,
                Message = message
            })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}