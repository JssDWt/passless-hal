using System;
namespace Passless.AspNetCore.Hal.Inspectors
{
    public interface IHalResourceInspectorMetadata
    {
        bool UseOnEmbeddedResources { get; }
        bool UseOnRootResource { get; }
    }
}
