using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;

namespace Passless.AspNetCore.Hal.Extensions
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
                // Add links using attributes.
                var linkAttributeInspector =
                    ActivatorUtilities.CreateInstance<AttributeLinkInspector>(serviceProvider);

                options.ResourceInspectors.Add(linkAttributeInspector);

                // Add embedded resources using attributes.
                var embedAttributeInspector =
                    ActivatorUtilities.CreateInstance<AttributeEmbedInspector>(serviceProvider);

                options.ResourceInspectors.Add(embedAttributeInspector);

                // Validate the resulting resources.
                var validationInspector =
                    ActivatorUtilities.CreateInstance<ResourceValidationInspector>(serviceProvider);
                options.ResourceInspectors.Insert(0, validationInspector);

                // Check permissions for accessing links.
                var permissionInspector =
                    ActivatorUtilities.CreateInstance<LinkPermissionInspector>(serviceProvider);
                options.ResourceInspectors.Add(permissionInspector);
            }
        }
    }
}
