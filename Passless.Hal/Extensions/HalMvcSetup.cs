using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Passless.Hal.Formatters;
using Passless.Hal.Internal;

namespace Passless.Hal.Extensions
{
    public class HalMvcSetup : IConfigureOptions<MvcOptions>, IPostConfigureOptions<MvcOptions>
    {
        private readonly MvcJsonOptions jsonOptions;
        private readonly HalOptions halOptions;
        private readonly ArrayPool<char> charPool;

        public HalMvcSetup(
            IOptions<HalOptions> halOptions,
            IOptions<MvcJsonOptions> jsonOptions,
            ArrayPool<char> charPool)
        {
            this.halOptions = halOptions?.Value
                ?? throw new ArgumentNullException(nameof(halOptions));
            this.jsonOptions = jsonOptions?.Value 
                ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.charPool = charPool 
                ?? throw new ArgumentNullException(nameof(charPool));
        }

        public void Configure(MvcOptions options)
        {
            // Register hal json output formatter before the regular 
            // json formatter, otherwise the regular one would consume
            // 'application/hal+json' requests.
            options.OutputFormatters.Insert(
                0,
                new HalJsonOutputFormatter(
                    this.jsonOptions.SerializerSettings,
                    this.charPool,
                    this.halOptions));
        }

        public void PostConfigure(string name, MvcOptions options)
        {
            options.Filters.AddService<LinkValidationFilter>();
        }
    }
}
