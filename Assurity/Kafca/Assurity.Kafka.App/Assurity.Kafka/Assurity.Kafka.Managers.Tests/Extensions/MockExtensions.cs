namespace Assurity.Kafka.Managers.Tests.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Logging;
    using Moq;

    [ExcludeFromCodeCoverage]
    public static class MockExtensions
    {
        /// <summary>
        /// Allows Moq to verify logs made with Microsoft's ILogger extension.
        /// When setting up a unit test, the ILogger mock must not use MockBehavior.Strict and must not use Setup.
        /// <para>
        /// References:
        /// https://stackoverflow.com/questions/66307477/how-to-verify-iloggert-log-extension-method-has-been-called-using-moq.
        /// https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The class where the log was made.</typeparam>
        /// <param name="logger"></param>
        /// <param name="expectedLogLevel"></param>
        /// <param name="expectedMessage"></param>
        /// <param name="times">Defaults to Once if not provided.</param>
        /// <returns></returns>
        public static Mock<ILogger<T>> VerifyLog<T>(
            this Mock<ILogger<T>> logger,
            LogLevel expectedLogLevel,
            string expectedMessage,
            Times? times = null)
        {
            times ??= Times.Once();

            // If no expectedMessage passed in, just return true.
            Func<object, Type, bool> state = (v, t) =>
                string.IsNullOrEmpty(expectedMessage) || v.ToString().Contains(expectedMessage);

            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == expectedLogLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                (Times)times);

            return logger;
        }
    }
}