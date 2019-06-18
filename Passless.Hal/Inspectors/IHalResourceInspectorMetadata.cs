using System;
namespace Passless.Hal.Inspectors
{
    public interface IHalResourceInspectorMetadata
    {
        bool UseOnEmbeddedResources { get; }
        bool UseOnRootResource { get; }
    }
}
