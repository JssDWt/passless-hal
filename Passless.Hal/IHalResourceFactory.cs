using System;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal.Extensions;

namespace Passless.Hal
{
    public interface IHalResourceFactory : IHalResourceFactoryMetadata
    {
        IResource CreateResource(ResourceFactoryContext context);
    }
}
