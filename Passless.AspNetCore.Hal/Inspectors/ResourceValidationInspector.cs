using System;
using System.Collections.Generic;

namespace Passless.AspNetCore.Hal.Inspectors
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

            var linkCounts = new Dictionary<string, int>();
            foreach (var link in context.Resource.Links)
            {
                if (linkCounts.ContainsKey(link.Rel))
                {
                    linkCounts[link.Rel]++;
                }
                else
                {
                    linkCounts[link.Rel] = 1;
                }
            }

            foreach (var count in linkCounts)
            {
                if (context.Resource.SingularRelations.Contains(count.Key)
                    && count.Value > 1)
                {
                    throw new HalException($"Relation '{count.Key}' is marked as singular, but contains {count.Value} links.");
                }
            }

            var embeddedCounts = new Dictionary<string, int>();
            foreach (var embedded in context.Resource.Embedded)
            {
                if (!linkCounts.ContainsKey(embedded.Rel))
                {
                    throw new HalException($"Resource contains embedded resource with rel '{embedded.Rel}', but not corresponding link exists.");
                }

                if (embeddedCounts.ContainsKey(embedded.Rel))
                {
                    embeddedCounts[embedded.Rel]++;
                }
                else
                {
                    embeddedCounts[embedded.Rel] = 1;
                }
            }

            foreach (var count in embeddedCounts)
            {
                if (context.Resource.SingularRelations.Contains(count.Key)
                    && count.Value > 1)
                {
                    throw new HalException($"Relation '{count.Key}' is marked as singular, but contains {count.Value} embedded resources.");
                }
            }
        }

        public void OnInspectingResource(HalResourceInspectingContext context)
        {

        }
    }
}
