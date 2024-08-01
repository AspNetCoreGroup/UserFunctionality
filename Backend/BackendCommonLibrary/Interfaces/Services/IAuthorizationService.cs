namespace BackendCommonLibrary.Interfaces.Services
{
    public interface IAuthorizationService
    {
        public Task<int> GetAuthorisedUserIDAsync(string token);
    }
}