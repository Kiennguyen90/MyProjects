using System.Diagnostics;

namespace UserManagementAPI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                Debug.WriteLine($"Request Path: {context.Request.Path}");
                Debug.WriteLine($"Request Method: {context.Request.Method}");
                await _next(context);
                Debug.WriteLine($"Response Status Code: {context.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
