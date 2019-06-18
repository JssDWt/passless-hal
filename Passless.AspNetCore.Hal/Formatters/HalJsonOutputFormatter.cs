using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Passless.AspNetCore.Hal.Converters;
using Passless.AspNetCore.Hal.Extensions;

namespace Passless.AspNetCore.Hal.Formatters
{
    public class HalJsonOutputFormatter : JsonOutputFormatter, IHalFormatter
    {
        public HalJsonOutputFormatter(
            JsonSerializerSettings serializerSettings, 
            ArrayPool<char> charPool, 
            HalOptions options)
            : base(serializerSettings, charPool)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.SupportedMediaTypes.Clear();
            if (options.SupportedMediaTypes != null)
            {
                foreach (var mediaType in options.SupportedMediaTypes)
                {
                    this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
                }
            }
        }

        protected override JsonSerializer CreateJsonSerializer()
        {
            var serializer = base.CreateJsonSerializer();

            var resourceConverter = new ResourceJsonConverter();
            serializer.Converters.Add(resourceConverter);

            return serializer;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetRequiredService<ILogger<HalJsonOutputFormatter>>();

            logger.LogDebug("Start serializing HAL response body.");
            using (var textWriter = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            using (var writer = this.CreateJsonWriter(textWriter))
            {
                var serializer = this.CreateJsonSerializer();
                serializer.Serialize(writer, context.Object);
                writer.Flush();
            }

            logger.LogDebug("End serializing HAL response body.");
            return Task.CompletedTask;
        }
    }
}
