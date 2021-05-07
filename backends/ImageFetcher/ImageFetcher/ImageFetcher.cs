using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ImageFetcher.Services;
using System.Linq;

namespace ImageFetcher
{
    public class ImageFetcher
    {
        private readonly IDlinkImageService _service;

        public ImageFetcher(IDlinkImageService service)
        {
            _service = service;
        }

        [FunctionName("currentImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "get",
            Route = null)] HttpRequest req,
            ILogger log)
        {
            string token = req.Query["token"];
            log.LogInformation($"Incoming token {token}");

            if (!IsValidToken(token))
            {
                return new UnauthorizedResult();
            }

            var result = await _service.GetImageBytesAsync();
            if (result.IsSuccessCode)
            {
                return new FileContentResult(result.Data, "image/jpeg");
            }
            else
            {
                return new ContentResult
                {
                    StatusCode = result.Code,
                    Content = result.Message
                };
            }
        }

        private static bool IsValidToken(string token)
        {
            // TODO: Load from configuration
            string[] validTokens = { "xxx", "xxx", "xxx" };
            return !string.IsNullOrEmpty(token)
                && validTokens.Contains(token);
        }
    }
}
