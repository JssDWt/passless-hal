using System;
using System.Threading.Tasks;

namespace Passless.Hal
{
    public delegate Task<HalResourceInspectedContext> HalResourceInspectionDelegate();
}
