using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Extensions;

namespace Passless.AspNetCore.Hal.Factories
{
    public interface IAsyncHalResourceFactory : IHalResourceFactoryMetadata
    {
        Task<IResource> CreateResourceAsync(ResourceFactoryContext context);
    }
}
