using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Internal
{
    public interface IUriService<T>
    {
        string GetUri(T attribute, IUrlHelper urlHelper);
        string GetUri(T attribute, IUrlHelper urlHelper, object obj);
    }
}
