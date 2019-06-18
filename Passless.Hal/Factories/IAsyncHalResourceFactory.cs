using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal.Extensions;

namespace Passless.Hal.Factories
{
    public interface IAsyncHalResourceFactory : IHalResourceFactoryMetadata
    {
        Task<IResource> CreateResourceAsync(ResourceFactoryContext context);
    }
}
