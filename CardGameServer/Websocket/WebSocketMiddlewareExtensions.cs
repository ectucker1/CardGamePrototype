namespace CardGameServer.Websocket;

public static class WebSocketMiddlewareExtensions
{
    public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
        PathString path,
        Func<HttpContext, WebSocketHandler?> getHandler)
    {
        return app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments(path))
            {
                await new WebSocketManagerMiddleware(next, getHandler(context)).Invoke(context);
            }
            else
            {
                await next(context);
            }
        });
    }
    
    public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
    {
        services.AddTransient<ConnectionManager>();
        return services;
    }
}