using System;
namespace Passless.AspNetCore.Hal.Attributes
{
    public interface IHalAttribute
    {
        string Rel { get; }
        bool IsSingular { get; set; }
    }
}
