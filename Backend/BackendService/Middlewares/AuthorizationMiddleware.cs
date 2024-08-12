using BackendCommonLibrary.Interfaces.Services;

namespace BackendService.Middlewares
{
    public class AuthorizationMiddleware
    {
        private RequestDelegate Next { get; }

        private ILogger Logger { get; }

        private IAuthorizationService AuthorizationService { get; }


        public AuthorizationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IAuthorizationService authorizationService)
        {
            Next = next;
            Logger = loggerFactory.CreateLogger<AuthorizationMiddleware>();

            AuthorizationService = authorizationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Items["auth_token"]?.ToString() ?? "";
            var userID = await AuthorizationService.GetAuthorisedUserIDAsync(token);

            context.Items["requestingUserID"] = userID;

            await Next.Invoke(context);
        }
    }
}

