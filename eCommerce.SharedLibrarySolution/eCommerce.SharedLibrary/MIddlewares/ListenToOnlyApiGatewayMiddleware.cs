using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.MIddlewares
{
    /// <summary>
    /// We want all the services like Authentication, Product and Orders to only listen to 
    /// requests from api gateway. 
    /// </summary>
    /// <param name="next"></param>
    public class ListenToOnlyApiGatewayMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            /*
             * As the client request comes to API Gateway, we will add something to the header
             * by intercepting the request. 
             * This header will be available to then all the further services. 
             */

            // Extract specific header from the request. 
            var signedHeader = httpContext.Request.Headers["Api-Gateway"];

            // NULL means that the request is not coming to the service from the api gateway. 
            if (signedHeader.FirstOrDefault() == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await httpContext.Response.WriteAsync("Service is unavailable...Come using proper channels.");
                return;
            }
            else
            {
                await next(httpContext);
            }
        }
    }
}
