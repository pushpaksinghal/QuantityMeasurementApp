using Microsoft.AspNetCore.Mvc.Filters;

namespace QuantityMeasurementApp.APILayer.Filters
{
    public class ActionLoggingFilter : IActionFilter
    {
        private readonly ILogger<ActionLoggingFilter> _logger;

        public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string controller = context.RouteData.Values["controller"]?.ToString() ?? "UnknownController";
            string action = context.RouteData.Values["action"]?.ToString() ?? "UnknownAction";

            _logger.LogInformation("Starting action {Controller}/{Action}", controller, action);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string controller = context.RouteData.Values["controller"]?.ToString() ?? "UnknownController";
            string action = context.RouteData.Values["action"]?.ToString() ?? "UnknownAction";

            _logger.LogInformation("Completed action {Controller}/{Action}", controller, action);
        }
    }
}