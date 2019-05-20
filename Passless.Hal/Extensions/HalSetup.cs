using System;
using Microsoft.Extensions.Options;

namespace Passless.Hal.Extensions
{
    public class HalSetup : IConfigureOptions<HalOptions>
    {
        private IHalResourceFactoryMetadata resourceFactory;
        public HalSetup(IHalResourceFactoryMetadata resourceFactory)
        {
            this.resourceFactory = resourceFactory
                ?? throw new ArgumentNullException(nameof(resourceFactory));
        }

        public void Configure(HalOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.ResourceFactories.Count == 0)
            {
                options.ResourceFactories.Add(resourceFactory);
            }
        }
    }
}
