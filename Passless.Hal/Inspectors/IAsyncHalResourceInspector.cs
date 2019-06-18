using System;
using System.Threading.Tasks;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public interface IAsyncHalResourceInspector : IHalResourceInspectorMetadata
    {
        Task<HalResourceInspectedContext> OnResourceInspectionAsync(HalResourceInspectingContext context, HalResourceInspectionDelegate next);
    }
}
