using System;
using System.Linq;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Inspectors;

namespace Passless.AspNetCore.Hal.Internal
{
    public class ResourceInspectorSelector
    {
        private readonly HalOptions options;

        public ResourceInspectorSelector(HalOptions options)
        {
            this.options = options
                ?? throw new ArgumentNullException(nameof(options));
        }

        public IHalResourceInspectorMetadata[] Select(bool isRootResource)
        {
            var inspectors = this.options.ResourceInspectors.Where(i => i != null);
            if (isRootResource)
            {
                inspectors = inspectors.Where(i => i.UseOnRootResource);
            }
            else
            {
                inspectors = inspectors.Where(i => i.UseOnEmbeddedResources);
            }

            return inspectors.ToArray();
        }
    }
}
