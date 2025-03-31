using Microsoft.EntityFrameworkCore;
using Mnemosyne.Data;

namespace Mnemosyne.Security
{
    public class AuthKeyMiddleware(AppDbContext dbContext, Serilog.ILogger log) : IMiddleware
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly Serilog.ILogger _logger = log;
        

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var providedKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key is required.");
                return;
            }

            var validKey = await _dbContext.ApiKeys
                .AnyAsync(k => k.Key == providedKey && k.Expires > DateTime.UtcNow);

            if (!validKey)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("API key is invalid or expired.");
                _logger.Debug("Attempt to use an expired api key {providedKey}", providedKey);
                return;
            }
            _logger.Debug("Request {method} {path} successfully authorized for key {key}", context.Request.Method, context.Request.Path, providedKey);
            await next(context);
        } // InvokeAsync
    } // class AuthKeyMiddleware
} // namespace

