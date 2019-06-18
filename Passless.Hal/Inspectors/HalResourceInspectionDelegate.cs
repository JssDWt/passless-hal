using System;
using System.Threading.Tasks;

namespace Passless.Hal.Inspectors
{
    public delegate Task<HalResourceInspectedContext> HalResourceInspectionDelegate();
}
