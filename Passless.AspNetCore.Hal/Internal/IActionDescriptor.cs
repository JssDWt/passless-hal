using System;
namespace Passless.AspNetCore.Hal.Internal
{
    public interface IActionDescriptor
    {
        string Action { get; }
        string Controller { get; set; }
        string Parameter { get; set; }
    }
}
