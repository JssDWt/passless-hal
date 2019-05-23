using System;
using System.Threading.Tasks;

namespace Passless.Hal
{
    public interface IAsyncHalResourceInspector : IHalResourceInspectorMetadata
    {
        Task<HalResourceInspectedContext> OnResourceInspectionAsync(HalResourceInspectingContext context, HalResourceInspectionDelegate next);
    }
}
