using CommonLibrary.Interfaces.Services;
using BackendService.DataSources;

namespace BackendService.Services
{
    public class SimpleAuthorizationService : IAuthorizationService
    {
        private BackendContext Context { get; set; }

        private Dictionary<string, int> UsersTokens { get; set; }


        public SimpleAuthorizationService(BackendContext context)
        {
            Context = context;

            UsersTokens = new Dictionary<string, int>();
        }

        public async Task RegisterAsync(string login, string password)
        {
            await Task.CompletedTask;
        }

        public async Task RegisterConfirmationAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            await Task.CompletedTask;

            var user = Context.Users.FirstOrDefault(x => x.UserLogin == login && x.UserPassword == password)
                ?? throw new Exception("Invalid login or password.");

            var token = Guid.NewGuid().ToString();

            lock (UsersTokens)
            {
                UsersTokens.Add(token, user.UserID);
            }

            return token;
        }

        public async Task LogOutAsync(string token)
        {
            await Task.CompletedTask;

            lock (UsersTokens)
            {
                UsersTokens.Remove(token);
            }
        }

        public async Task<int> GetAuthorisedUserIDAsync(string token)
        {
            await Task.CompletedTask;

            return UsersTokens[token];
        }
    }
}