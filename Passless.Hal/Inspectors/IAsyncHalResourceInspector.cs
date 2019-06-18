using System;
using System.Threading.Tasks;

namespace Passless.Hal.Inspectors
{
    public interface IAsyncHalResourceInspector : IHalResourceInspectorMetadata
    {
        Task<HalResourceInspectedContext> OnResourceInspectionAsync(HalResourceInspectingContext context, HalResourceInspectionDelegate next);
    }
}
