using ImageFetcher.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ImageFetcherTests
{
    public class DLinkImageServiceTests
    {
        private readonly ILogger<DlinkImageService> _logger;

        public DLinkImageServiceTests()
        {
            _logger = Substitute.For<ILogger<DlinkImageService>>();
        }

        public static IEnumerable<object[]> InvalidConfiguration => new List<object[]>
        {
            new object[] { 
                new Dictionary<string, string>
                {
                    {"BaseUrl", ""}
                }
            },
            new object[] { new Dictionary<string, string>() }
        };

        [Fact]
        public void InitializeWithConfiguration_ReturnInstance()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"BaseUrl", "https://127.0.0.1"},
                { "BasicAuth", "user:password"}
            };
            var configuration = CreateConfiguration(dictionary);
            var httpClient = Substitute.For<HttpClient>();

            IDlinkImageService service = new DlinkImageService(configuration, httpClient, _logger);

            Assert.NotNull(service);
        }

        [Theory]
        [MemberData(nameof(InvalidConfiguration))]
        public void InitializeWithInvalidConfiguration_ThrowsException(Dictionary<string, string> dictionary)
        {
            var configuration = CreateConfiguration(dictionary);
            var httpClient = Substitute.For<HttpClient>();

            Action action = () => new DlinkImageService(configuration, httpClient, _logger);

            Assert.Throws<ConfigurationErrorsException>(action);
        }

        [Fact]
        public void InitializeWithoutConfiguration_ThrowsException()
        {
            var dictionary = new Dictionary<string, string>();
            var configuration = CreateConfiguration(dictionary);
            var httpClient = Substitute.For<HttpClient>();

            Action action = () => new DlinkImageService(configuration, httpClient, _logger);

            Assert.Throws<ConfigurationErrorsException>(action);
        }

        [Fact]
        public async Task GetImage_ReturnsImageAsync()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"BaseUrl", "https://127.0.0.1"},
                { "BasicAuth", "user:password"}
            };
            var configuration = CreateConfiguration(dictionary);
            var messageHandler = new MockedMessageHandler();
            messageHandler.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(new byte[10])
            };

            var httpClient = new HttpClient(messageHandler);

            IDlinkImageService service = new DlinkImageService(configuration, httpClient, _logger);

            var response = await service.GetImageBytesAsync();

            Assert.Equal((int) HttpStatusCode.OK, response.Code);
            Assert.NotNull(response.Data);
        }

        private IConfiguration CreateConfiguration(Dictionary<string, string> dictionary)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(dictionary)
                .Build();
            return configurationRoot;
        }

        public class MockedMessageHandler : HttpMessageHandler
        {
            public HttpResponseMessage Response { get; set; }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Response;
            }
        }
    }
}
