using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Passless.Hal.Factories;
using Passless.Hal.Inspectors;

namespace Passless.Hal.Extensions
{
    public class HalSetup : IConfigureOptions<HalOptions>
    {
        private IHalResourceFactoryMetadata resourceFactory;
        private IServiceProvider serviceProvider;
        public HalSetup(
            IServiceProvider serviceProvider, 
            IHalResourceFactoryMetadata resourceFactory)
        {
            this.resourceFactory = resourceFactory
                ?? throw new ArgumentNullException(nameof(resourceFactory));

            this.serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Configure(HalOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.UseDefaultResourceFactory)
            {
                options.ResourceFactory = resourceFactory;
            }

            if (options.UseDefaultResourceInspectors)
            {
                var embedAttributeInspector =
                    ActivatorUtilities.CreateInstance<AttributeEmbedHalResourceInspector>(serviceProvider);

                options.ResourceInspectors.Add(embedAttributeInspector);

                var linkAttributeInspector =
                    ActivatorUtilities.CreateInstance<AttributeLinkInspector>(serviceProvider);

                options.ResourceInspectors.Add(linkAttributeInspector);
            }

        }
    }
}
