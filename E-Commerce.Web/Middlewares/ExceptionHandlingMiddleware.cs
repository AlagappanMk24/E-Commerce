namespace E_Commerce.Web.Middlewares
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
        private readonly IWebHostEnvironment _env = env;
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception occurred. TraceId: {TraceId}",
                    context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Something went wrong. Please try again later.",
                TraceId = context.TraceIdentifier,
                Details = _env.IsDevelopment() ? exception.Message : null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}