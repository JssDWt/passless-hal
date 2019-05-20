using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Passless.Hal;
using Passless.Hal.Extensions;

namespace Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddHal(options =>
                {
                    options.SupportedMediaTypes.Add("application/vnd-hal+json");

                    // Optionally add custom resource factories here, in order to
                    // create resources from your returned actionresults.
                    // By default, the AttributeEmbedHalResourceFactory is present.
                    // options.ResourceFactories.Add(new MyCustomResourceFactory());
                });

            // Instead of adding a ResourceFactory in the AddHal method, you
            // can create a class that implements IConfigureOptions<HalOptions>.
            // Then call services.ConfigureOptions<MyCustomHalOptionsSetup>(); here.
            // This would allow you to leverage dependency injection inside the
            // resourcefactory, by injecting classes into your IConfigureOptions
            // and then setting the resourcefactory in the Configure method.
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            // UseHal should be before UseMvc.
            // Note that this middleware will actually write the response instead
            // of the UseMvc method if it is present.
            app.UseHal();
            app.UseMvcWithDefaultRoute();
        }
    }
}
