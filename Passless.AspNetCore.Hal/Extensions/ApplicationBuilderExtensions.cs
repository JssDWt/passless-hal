using System;
using Microsoft.AspNetCore.Builder;

namespace Passless.AspNetCore.Hal.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHal(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HalMiddleware>();
            return builder;
        }
    }
}
