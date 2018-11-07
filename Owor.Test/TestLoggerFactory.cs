using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;

namespace Owor.Test
{

    public class TestLoggerFactory : ILoggerFactory
    {

        public List<Mock<ILogger>> MockedLoggers = new List<Mock<ILogger>>();

        public void AddProvider(ILoggerProvider provider)
        {
            // ...
        }

        public ILogger CreateLogger(string categoryName)
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Name = $"LoggerMock:{categoryName}";

            if (categoryName.StartsWith("Owor"))
                MockedLoggers.Add(loggerMock);

            return loggerMock.Object;
        }

        public void Dispose()
        {
            VerifyNoErrorsWereLogged();
        }

        private void VerifyNoErrorsWereLogged()
        {
            foreach (var loggerMock in MockedLoggers)
            {
                loggerMock.Verify(l => l.Log(LogLevel.Error, 0, It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()), Times.Never);
            }
        }

    }

}