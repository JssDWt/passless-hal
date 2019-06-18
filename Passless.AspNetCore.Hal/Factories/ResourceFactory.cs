using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Factories
{
    public delegate Task<IResource> ResourceFactory(ActionContext context, object resource);
}
