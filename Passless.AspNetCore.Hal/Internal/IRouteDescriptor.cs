using System;
namespace Passless.AspNetCore.Hal.Internal
{
    public interface IRouteDescriptor
    {
        string RouteName { get; }
        string Parameter { get; set; }
    }
}
