using System;
using System.Threading.Tasks;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public delegate Task<HalResourceInspectedContext> HalResourceInspectionDelegate();
}
