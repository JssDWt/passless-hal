using System;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal.Extensions;

namespace Passless.Hal.Factories
{
    public interface IHalResourceFactory : IHalResourceFactoryMetadata
    {
        IResource CreateResource(ResourceFactoryContext context);
    }
}
