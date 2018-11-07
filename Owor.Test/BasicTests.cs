using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Owor.All;
using Owor.ClientLib.Devices;
using Xunit;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Abstractions;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Owor.Test
{
    public class BasicTests : TestBase, IClassFixture<WebApplicationFactory<Startup>>
    {
        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            var internalDeviceClient = factory.WithWebHostBuilder(builder =>
            {
                var rootPath = "/test";
                var testFileSystem = GetTestFileSystem(rootPath);
                
                builder.ConfigureAppConfiguration((hostingContext, configBuilder) => {
                    configBuilder.AddInMemoryCollection(new[] {
                        new KeyValuePair<string, string>("OwConfig:BasePath", rootPath)
                    });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ILoggerFactory, TestLoggerFactory>();
                    services.AddTransient<IFileSystem>((svp) => testFileSystem);
                });

            }).CreateClient();

            _httpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ILoggerFactory, TestLoggerFactory>();
                    services.AddTransient<IDevicesClient>((svp) => new DevicesClient(svp.GetRequiredService<ILogger<DevicesClient>>(), internalDeviceClient));
                });
            })
            .CreateClient();

            _factory = factory;
        }

        [Theory]
        [InlineData("/Overview")]
        [InlineData("/Overview/About")]
        public async Task EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // ARRANGE

            // ACT
            var response = await _httpClient.GetAsync(url);

            // ASSERT
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task OverviewContainsConfiguredDevices()
        {
            // ARRANGE

            // ACT
            var response = await _httpClient.GetAsync("/Overview");

            // ASSERT
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var document = new HtmlParser().Parse(content);
            Assert.Equal("Overview", document.QuerySelector("h2").TextContent);

            VerifyNoErrorWasLogged();
        }

    }
    
}
