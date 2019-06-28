using Microsoft.AspNetCore.Builder;

namespace Passless.AspNetCore.Hal.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds HAL middleware to the request pipeline.
        /// </summary>
        /// <param name="builder">The applicationbuilder.</param>
        /// <returns>The applicationbuilder.</returns>
        /// <remarks>
        /// Using hal middleware will intercept any response from the MVC pipeline and
        /// converts it to a HAL resource. It allows to create <see cref="Inspectors.IHalResourceInspector"/> and
        /// <see cref="Inspectors.IAsyncHalResourceInspector"/> instances to inspect and modify the returned resource.
        /// </remarks>
        public static IApplicationBuilder UseHal(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HalMiddleware>();
            return builder;
        }
    }
}
