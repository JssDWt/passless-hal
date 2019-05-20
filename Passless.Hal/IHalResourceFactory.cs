using System;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal.Extensions;

namespace Passless.Hal
{
    public interface IHalResourceFactory : IHalResourceFactoryMetadata
    {
        void CreateResource(HalMiddleware middleware, ActionContext actionContext, ObjectResult objectResult);
    }
}
