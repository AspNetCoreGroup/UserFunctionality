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
            try
            {
                var token = context.Items["token"]?.ToString() ?? "";
                var userID = AuthorizationService.GetAuthorisedUserIDAsync(token);

                context.Items["requestUserID"] = userID;

                await Next.Invoke(context);
            }
            catch
            {
                context.Response.StatusCode = 403;
            }
        }
    }
}

