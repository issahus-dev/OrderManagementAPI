using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrderManagementAPI.Security
{
    
        /// <summary>
        /// Handle exceptions by returning the consumer: Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError without further details. 
        /// Logs errors in the background.
        /// </summary>
        internal class CustomExceptionFilterAttribute : ExceptionFilterAttribute
        {
            private ILogger<CustomExceptionFilterAttribute> Logger { get; set; }

            public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
            {
                Logger = logger ?? throw new ArgumentNullException("logger");
            }

            public override void OnException(ExceptionContext context)
            {
                IActionResult result = new StatusCodeResult(500);
                Logger.LogError(default(EventId), context.Exception, "Exception thrown that was not handled:");
                context.Result = result;
            }

            public override async Task OnExceptionAsync(ExceptionContext context)
            {
                await Task.CompletedTask;
                IActionResult result = new StatusCodeResult(500);
                Logger.LogError(default(EventId), context.Exception, "Exception thrown that was not handled:");
                context.Result = result;
            }
        }
    }

