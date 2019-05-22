using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Helpers;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Client.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
        {
            if (httpContext.Request.Cookies.TryGetValue("jwtCookie", out var jwtToken))
                httpContext.Request.Headers.TryAdd("Authorization", "Bearer " + jwtToken);

            await _next(httpContext);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static void JwtMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }
    }
}
