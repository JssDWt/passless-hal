using System;
using System.Buffers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Passless.Hal.Filters;
using Passless.Hal.Formatters;

namespace Passless.Hal.Extensions
{
    public class HalMvcSetup : IConfigureOptions<MvcOptions>
    {
        private readonly MvcJsonOptions jsonOptions;
        private readonly HalOptions halOptions;
        private readonly ArrayPool<char> charPool;
        private readonly IUrlHelperFactory urlHelperFactory;

        public HalMvcSetup(
            IOptions<HalOptions> halOptions,
            IOptions<MvcJsonOptions> jsonOptions,
            ArrayPool<char> charPool,
            IUrlHelperFactory urlHelperFactory)
        {
            this.halOptions = halOptions?.Value
                ?? throw new ArgumentNullException(nameof(halOptions));
            this.jsonOptions = jsonOptions?.Value 
                ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.charPool = charPool 
                ?? throw new ArgumentNullException(nameof(charPool));
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
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
                    this.halOptions,
                    this.urlHelperFactory));
        }
    }
}
