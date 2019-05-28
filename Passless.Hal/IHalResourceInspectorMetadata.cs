using System;
namespace Passless.Hal
{
    public interface IHalResourceInspectorMetadata
    {
        bool UseOnEmbeddedResources { get; }
        bool UseOnRootResource { get; }
    }
}
