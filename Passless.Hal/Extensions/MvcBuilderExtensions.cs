using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Passless.Hal.Extensions
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddHal(this IMvcBuilder builder)
            => AddHal(builder, null);

        public static IMvcBuilder AddHal(
            this IMvcBuilder builder,
            Action<HalOptions> halOptionsBuilder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (halOptionsBuilder == null)
            {
                halOptionsBuilder = o => { };
            }

            // Make sure the custom formatters can access the action context.
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Add the options to HalMvcSetup
            builder.Services.Configure(halOptionsBuilder);

            // registers the custom formatter(s)
            builder.Services.ConfigureOptions<HalMvcSetup>();

            // Wrap the existing actionresultexecutor in the hal executor.
            builder.Services.Decorate<IActionResultExecutor<ObjectResult>, HalObjectResultExecutor>();

            return builder;
        }
    }
}
