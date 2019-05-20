using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests
{
    public class Constants
    {
        public static readonly ILoggerFactory LoggerFactory = new ServiceCollection()
            .AddLogging(logging =>
            {
                logging.AddDebug();
            })
            .BuildServiceProvider()
            .GetService<ILoggerFactory>();
    }
}
