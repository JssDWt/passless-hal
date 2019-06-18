using System;
namespace Passless.Hal.Inspectors
{
    public interface IHalResourceInspector : IHalResourceInspectorMetadata
    {
        void OnInspectingResource(HalResourceInspectingContext context);
        void OnInspectedResource(HalResourceInspectedContext context);
    }
}
