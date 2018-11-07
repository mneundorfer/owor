using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Owor.All;

namespace Owor.Test
{

    public class TestBase
    {

        protected HttpClient _httpClient;
        protected WebApplicationFactory<Startup> _factory;

        protected MockFileSystem GetTestFileSystem(string rootPath)
        {
            var testFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{rootPath}/23-memchip/eeprom", new MockFileData("TODO") },
                { $@"{rootPath}/10-temperaturesensor/w1_slave", new MockFileData("31 00 4b 46 ff ff 05 10 1c : crc=1c YES\n31 00 4b 46 ff ff 05 10 1c t=24437") },
                { $@"{rootPath}/26-humiditysensor/vdd", new MockFileData("512") },
                { $@"{rootPath}/26-humiditysensor/vad", new MockFileData("319") }
            });

            return testFileSystem;
        }

        private void VerifyNothingOnLevelWasLogged(params LogLevel[] logLevel)
        {
            foreach (var fac in _factory.Factories)
            {
                foreach (var loggerMock in (fac.Server.Host.Services.GetRequiredService<ILoggerFactory>() as TestLoggerFactory).MockedLoggers)
                {
                    foreach (var level in logLevel)
                    {
                        loggerMock.Verify(l => l.Log(level, 0, It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()), Times.Never);
                    }
                }
            }
        }

        protected void VerifyNoErrorWasLogged()
        {
            VerifyNothingOnLevelWasLogged(LogLevel.Error);
        }

        protected void VerifyNoErrorAndWarningWereLogged()
        {
            VerifyNothingOnLevelWasLogged(LogLevel.Error, LogLevel.Warning);
        }

        protected void VerifyNoWarningWasLogged()
        {
            VerifyNothingOnLevelWasLogged(LogLevel.Warning);
        }

    }

}