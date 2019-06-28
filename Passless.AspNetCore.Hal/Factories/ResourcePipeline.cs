using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Factories
{
    public delegate Task<IResource> ResourcePipeline(ActionContext context, object resource);
}
