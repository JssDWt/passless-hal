using System;
namespace Passless.Hal
{
    public interface IHalResourceInspector : IHalResourceInspectorMetadata
    {
        void OnInspectingResource(HalResourceInspectingContext context);
        void OnInspectedResource(HalResourceInspectedContext context);
    }
}
