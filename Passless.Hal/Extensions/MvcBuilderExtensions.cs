using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.FeatureFlags;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Extensions
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

            AddHal(builder.Services, halOptionsBuilder);

            return builder;
        }

        public static IMvcCoreBuilder AddHal(this IMvcCoreBuilder builder)
            => AddHal(builder, null);

        public static IMvcCoreBuilder AddHal(
            this IMvcCoreBuilder builder,
            Action<HalOptions> halOptionsBuilder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            AddHal(builder.Services, halOptionsBuilder);

            return builder;
        }

        private static void AddHal(IServiceCollection services, Action<HalOptions> halOptionsBuilder)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Make sure the custom formatters can access the action context.
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Inject the default resource factory.
            services.TryAddSingleton<IHalResourceFactoryMetadata, DefaultHalResourceFactory>();
            services.ConfigureOptions<HalSetup>();

            if (halOptionsBuilder != null)
            {
                services.Configure(halOptionsBuilder);
            }

            // registers the custom formatter(s)
            services.ConfigureOptions<HalMvcSetup>();

            // Wrap the existing actionresultexecutor in the hal executor.
            services.Decorate<IActionResultExecutor<ObjectResult>, HalObjectResultExecutor>();
        }
    }
}
