using BackendCommonLibrary.Interfaces.Services;

namespace BackendService.Middlewares
{
    public class AuthorizationMiddleware
    {
        private RequestDelegate Next { get; }

        private ILogger Logger { get; }

        private IServiceProvider ServiceProvider { get; }


        public AuthorizationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            Next = next;
            Logger = loggerFactory.CreateLogger<AuthorizationMiddleware>();

            ServiceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //using var scope = context.
            //var authorizationService = ServiceProvider.GetService<IAuthorizationService>();

            var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();

            var token = context.Items["auth_token"]?.ToString() ?? "";
            var userID = await authorizationService.GetAuthorisedUserIDAsync(token);

            context.Items["requestingUserID"] = userID;

            await Next.Invoke(context);
        }
    }
}

