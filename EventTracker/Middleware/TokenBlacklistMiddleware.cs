using UserManagment.Service.Service;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "");
            if (await blacklistService.IsTokenBlacklistedAsync(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is revoked.");
                return;
            }
        }

        await _next(context);
    }
}
