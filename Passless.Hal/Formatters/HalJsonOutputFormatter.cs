using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Passless.Hal.Converters;
using Passless.Hal.Extensions;

namespace Passless.Hal.Formatters
{
    public class HalJsonOutputFormatter : JsonOutputFormatter
    {
        private HalOptions options;
        private IUrlHelperFactory urlHelperFactory;

        public HalJsonOutputFormatter(
            JsonSerializerSettings serializerSettings, 
            ArrayPool<char> charPool, 
            HalOptions options,
            IUrlHelperFactory urlHelperFactory)
            : base(serializerSettings, charPool)
        {
            this.options = options
                ?? throw new ArgumentNullException(nameof(options));

            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.SupportedMediaTypes.Clear();
            if (options.SupportedMediaTypes != null)
            {
                foreach (var mediaType in options.SupportedMediaTypes)
                {
                    this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
                }
            }
        }

        protected override bool CanWriteType(Type type)
        {
            // Called when IActionResult is returned
            return base.CanWriteType(type);
        }


        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            // Called when an object is returned from an action
            return base.CanWriteResult(context);
        }

        protected override JsonSerializer CreateJsonSerializer()
        {
            var serializer = base.CreateJsonSerializer();

            var resourceConverter = new ResourceJsonConverter();
            serializer.Converters.Add(resourceConverter);

            return serializer;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<HalJsonOutputFormatter>)) as ILogger;
            var actionContextAccessor = serviceProvider.GetService(typeof(IActionContextAccessor)) as IActionContextAccessor;

            object toSerialize = context.Object;
            if (this.options.ResourceFactory != null)
            {
                var urlHelper = this.urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
                toSerialize = await this.options.ResourceFactory(context.Object, actionContextAccessor.ActionContext, urlHelper);
            }

            using (var textWriter = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            using (var writer = this.CreateJsonWriter(textWriter))
            {
                var serializer = this.CreateJsonSerializer();
                serializer.Serialize(writer, toSerialize);
                writer.Flush();
            }
        }
    }
}
