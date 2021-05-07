
using ImageFetcher.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ImageFetcher.Startup))]
namespace ImageFetcher
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            services.AddHttpClient();
            services.AddLogging();
            services.AddTransient<IDlinkImageService, DlinkImageService>();
        }
    }
}
