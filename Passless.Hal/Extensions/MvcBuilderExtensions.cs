using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Passless.Hal.Factories;

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

            // Make sure the custom formatters can access the action context.
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Inject the default resource factory.
            builder.Services.AddSingleton<IHalResourceFactoryMetadata, DefaultHalResourceFactory>();
            builder.Services.ConfigureOptions<HalSetup>();

            if (halOptionsBuilder != null)
            {
                builder.Services.Configure(halOptionsBuilder);
            }

            // registers the custom formatter(s)
            builder.Services.ConfigureOptions<HalMvcSetup>();

            // Wrap the existing actionresultexecutor in the hal executor.
            builder.Services.Decorate<IActionResultExecutor<ObjectResult>, HalObjectResultExecutor>();

            return builder;
        }
    }
}
