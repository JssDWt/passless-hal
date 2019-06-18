using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public class HalResourceInspectorInvoker
    {
        private readonly int inspectorsLength;
        private readonly IHalResourceInspectorMetadata[] inspectors;
        private int currentIndex = 0;
        private IHalResourceInspectorMetadata currentInspector;
        private HalResourceInspectingContext inspectingContext;
        private readonly ILogger<HalResourceInspectorInvoker> logger;

        public HalResourceInspectorInvoker(
            IHalResourceInspectorMetadata[] inspectors,
            ILogger<HalResourceInspectorInvoker> logger)
        {
            this.inspectors = inspectors
                ?? new IHalResourceInspectorMetadata[0];

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this.inspectors = this.inspectors.Where(i => i != null).ToArray();
            this.inspectorsLength = this.inspectors.Length;
        }

        public async Task<HalResourceInspectedContext> InspectAsync(HalResourceInspectingContext context)
        {
            this.inspectingContext = context
                ?? throw new ArgumentNullException(nameof(context));
            this.currentIndex = 0;
            return await Next();
        }

        private async Task<HalResourceInspectedContext> Next()
        {
            // TODO: Validate the inspectingcontext here to make sure the last pipeline component did not do something illegal.

            // If the last inspector was invoked, create the inspectedcontext.
            if (currentIndex >= inspectorsLength)
            {
                return new HalResourceInspectedContext(inspectingContext);
            }

            HalResourceInspectedContext result = null;
            currentInspector = this.inspectors[currentIndex++];

            if (currentInspector is IAsyncHalResourceInspector asyncInspector)
            {
                result = await asyncInspector.OnResourceInspectionAsync(inspectingContext, Next);
            }
            else if (currentInspector is IHalResourceInspector syncInspector)
            {
                syncInspector.OnInspectingResource(inspectingContext);
                result = await Next();
                syncInspector.OnInspectedResource(result);
            }
            else
            {
                throw new InvalidOperationException(
                    $"inspector of type '{currentInspector.GetType().FullName}' is not an IHalResourceInspector or IAsyncHalResourceInspector.");
            }

            // TODO: Validate the inspectedcontext here to make sure the last pipeline component did not do something illegal.
            if (result == null)
            {
                throw new InvalidOperationException($"HalResourceInspectedContext was null after invoking '{currentInspector.GetType().FullName}'.");
            }

            return result;
        }
    }
}
