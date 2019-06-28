using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Passless.AspNetCore.Hal.Formatters;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Extensions
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
            if (halOptions == null)
            {
                throw new ArgumentNullException(nameof(halOptions));
            }

            this.halOptions = halOptions.Value
                ?? throw new ArgumentException("Value cannot be null.", nameof(halOptions));

            if (jsonOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonOptions));
            }

            this.jsonOptions = jsonOptions.Value 
                ?? throw new ArgumentException("Value cannot be null.", nameof(jsonOptions));

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
            options.Filters.AddService<LinkValidationFilter>(0);
        }
    }
}
