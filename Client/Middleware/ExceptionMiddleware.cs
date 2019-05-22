using System;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Client.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (FlurlHttpException ex)
            {
                
                logger.LogError(ex,
                    "Failed Http Call {Response} and {Reason}",
                    await ex.Call.Response?.Content.ReadAsStringAsync(),
                    ex.Call.Response?.ReasonPhrase);
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex ,"Something went wrong");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error - Go to Kibana for more information."
            }.ToString());
        }

    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static void ExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
