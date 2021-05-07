using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageFetcher.Services
{
    public interface IDlinkImageService
    {
        Task<ServiceResult<byte[]>> GetImageBytesAsync();
    }

    public class DlinkImageService : IDlinkImageService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DlinkImageService(IConfiguration configuration, 
            HttpClient httpClient,
            ILogger<DlinkImageService> logger)
        {
            _baseUrl = configuration["BaseUrl"];
            _httpClient = httpClient;
            _logger = logger;

            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new ConfigurationErrorsException("No 'BaseUrl' configured");
            }

            var usernameAndPassword = configuration["BasicAuth"];
            if (string.IsNullOrEmpty(usernameAndPassword))
            {
                throw new ConfigurationErrorsException("No 'BasicAuth' configured");
            }
            
            var usernameAndPasswordBytes = Encoding.ASCII.GetBytes(usernameAndPassword);
            var authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(usernameAndPasswordBytes));
            _httpClient.DefaultRequestHeaders.Authorization = authorization;
        }

        public async Task<ServiceResult<byte[]>> GetImageBytesAsync()
        {
            try
            {
                _logger.LogInformation("Getting image");
                var response = await _httpClient.GetAsync($"{_baseUrl}/image/jpeg.cgi");

                var bytes = await response.Content.ReadAsByteArrayAsync();

                return ServiceResult<byte[]>.Ok(bytes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception");
                return ServiceResult<byte[]>.Error(e.ToString());
            }
        }
    }
}
