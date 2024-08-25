using CommonLibrary.Extensions;
using BackendCommonLibrary.Interfaces.Services;
using IHttpClientFactory = CommonLibrary.Interfaces.Factories.IHttpClientFactory;

namespace BackendService.Services
{
    public class HTTPAuthorizationService : IAuthorizationService
    {
        private IHttpClientFactory HttpClientFactory { get; set; }
        private IConfiguration Configuration { get; set; }
        private ILogger Logger { get; set; }

        public HTTPAuthorizationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            HttpClientFactory = httpClientFactory;
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<HTTPAuthorizationService>();
        }

        public async Task<int> GetAuthorisedUserIDAsync(string token)
        {
            var httpClient = HttpClientFactory.GetHttpClient();

            var section = Configuration.GetSection("AuthorizationService") ?? throw new NullReferenceException("В конфиге не указано значение для AuthorizationService");
            var requestURL = section["RequestURL"] ?? throw new NullReferenceException("В конфиге не указано значение для AuthorizationService/RequestURL");

            var request = requestURL.ReflectionFormat(new { token });

            var result = await httpClient.GetAsync(request);

            result.EnsureSuccessStatusCode();

            var resultText = await result.Content.ReadAsStringAsync();

            return resultText.ParseToInt() ?? throw new FormatException("Ответ от сервиса авторизации получен в неверном формате. Ожидаемый формат: int");
        }
    }
}
