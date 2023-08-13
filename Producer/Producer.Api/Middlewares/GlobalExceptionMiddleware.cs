using System.Net;

namespace Producer.Api.Middlewares;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly NotificationContext _notification;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, NotificationContext notification)
    {
        _logger = logger;
        _notification = notification;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            context.Response.ContentType = "application/json";
            var statusCode = context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            _logger.LogError($"Exception Details: {message}");
            _notification.AddNotification(statusCode.ToString(), message);
            
            var response = new BaseResponse { Notifications = _notification.Notifications, };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}