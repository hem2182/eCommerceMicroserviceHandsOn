using eCommerce.SharedLibrary.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eCommerce.SharedLibrary.MIddlewares
{
    public class GlobalExceptionMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // First call all other middlewares
                await next(context);

                // Check if response here is TooManyRequests etc - 429
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    await ModifyHeader(context, "Warning", "Too Many Requests made", StatusCodes.Status429TooManyRequests);
                }

                // check if response is unauthorized - 401
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    await ModifyHeader(context, "Alert", "you are not authorized to access.", StatusCodes.Status401Unauthorized);

                // if response if forbidden - 403
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    await ModifyHeader(context, "Out of Access", "you are not allowed to access.", StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                // Log the exception to console, dubegger and file.
                LogException.LogExceptions(ex);

                // is timeout exception - 408
                if (ex is TaskCanceledException or TimeoutException)
                    await ModifyHeader(context, "Out of time", "Request timeout...Try again", StatusCodes.Status408RequestTimeout);
                // if none
                else
                    await ModifyHeader(context, "Error", "Oops! Internal server error occurred. Kindly try after somtime...", StatusCodes.Status500InternalServerError);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client. 
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }), CancellationToken.None);

            return;
        }
    }
}
