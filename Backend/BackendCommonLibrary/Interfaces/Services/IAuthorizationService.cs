namespace BackendCommonLibrary.Interfaces.Services
{
    public interface IAuthorizationService
    {
        public Task RegisterAsync(string login, string password);
        public Task RegisterConfirmationAsync();

        public Task<string> LoginAsync(string login, string password);
        public Task LogOutAsync(string token);

        public Task<int> GetAuthorisedUserIDAsync(string token);
    }
}