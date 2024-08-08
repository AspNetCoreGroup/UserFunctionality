using BackendCommonLibrary.Interfaces.Services;

namespace BackendService.Services
{
    /// <summary>
    /// Тестовая реализация сервиса авторизации. Токен авторизации для каждого пользователя представлен в формате "token:{UserID}".
    /// </summary>
    public class SimpleAuthorizationService : IAuthorizationService
    {
        public Task<int> GetAuthorisedUserIDAsync(string token)
        {
            try
            {
                return Task.FromResult(1);
                //return Task.FromResult(int.Parse(token.Split("token:")[1]));
            }
            catch
            {
                throw new Exception("Токен не зарегестрирован или истек.");
            }
        }
    }
}