using System;
using Microsoft.AspNetCore.Http;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public class MvcPipeline
    {
        public MvcPipeline(RequestDelegate pipeline)
            => this.Pipeline = pipeline
                ?? throw new ArgumentNullException(nameof(pipeline));

        public RequestDelegate Pipeline { get; }
    }
}
