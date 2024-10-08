using Serilog;
using System.Net;

namespace WebApplication2.Middleware
{
    public class GlobalExceptionHandlingMiddleWare
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<GlobalExceptionHandlingMiddleWare> _logger;

        public GlobalExceptionHandlingMiddleWare(ILogger<GlobalExceptionHandlingMiddleWare> logger, RequestDelegate requestDelegate)
        {
            _logger = logger;
            _requestDelegate = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext context) {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex) {
                Log.Error($"Something went wrong: {ex}");
                await HandleExceptionAsync(context,ex);

            }

        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errormessage = new
            {
                StatusCodes = (int)HttpStatusCode.InternalServerError,
                Message = "Internal Server Error from the custom middleware.",
                Detailed = exception.Message
            };
            return context.Response.WriteAsJsonAsync(errormessage);
        }

    }
}
