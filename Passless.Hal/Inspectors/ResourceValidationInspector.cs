using System;
using System.Collections.Generic;

namespace Passless.Hal.Inspectors
{
    public class ResourceValidationInspector : IHalResourceInspector
    {
        public bool UseOnEmbeddedResources => true;

        public bool UseOnRootResource => true;

        public void OnInspectedResource(HalResourceInspectedContext context)
        {
            if (context.Resource == null)
            {
                return;
            }

            var counts = new Dictionary<string, int>();
            foreach (var link in context.Resource.Links)
            {
                if (counts.ContainsKey(link.Rel))
                {

                }
            }
        }

        public void OnInspectingResource(HalResourceInspectingContext context)
        {

        }
    }
}
