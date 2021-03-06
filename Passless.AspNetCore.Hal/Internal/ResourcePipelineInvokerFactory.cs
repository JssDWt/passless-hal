﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Passless.AspNetCore.Hal.Inspectors;

namespace Passless.AspNetCore.Hal.Internal
{
    public class ResourcePipelineInvokerFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ResourcePipelineInvokerFactory()
        {

        }

        public ResourcePipelineInvokerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public virtual ResourcePipelineInvoker Create(MvcPipeline pipeline)
            => ActivatorUtilities.CreateInstance<ResourcePipelineInvoker>(serviceProvider, pipeline);
    }
}
