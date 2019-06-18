using System;
namespace Passless.AspNetCore.Hal.Inspectors
{
    public interface IHalResourceInspector : IHalResourceInspectorMetadata
    {
        void OnInspectingResource(HalResourceInspectingContext context);
        void OnInspectedResource(HalResourceInspectedContext context);
    }
}
