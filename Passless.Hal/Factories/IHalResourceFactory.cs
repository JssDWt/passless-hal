using System;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Extensions;

namespace Passless.AspNetCore.Hal.Factories
{
    public interface IHalResourceFactory : IHalResourceFactoryMetadata
    {
        IResource CreateResource(ResourceFactoryContext context);
    }
}
