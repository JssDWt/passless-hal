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
                    options.ResourceFactory = async (obj, context, urlHelper) =>
                    {
                        // This factory method (ResourceFactory) is called whenever an object will be serialized
                        // to return to the client. You can create IResource objects here,
                        // instead of inside your controllers, to decouple HAL from your controllers
                        // completely. This example sets the self link for every returned object.
                        // You can make this logic as complex as you'd want.

                        var resource = obj as IResource;
                        if (resource == null)
                        {
                            resource = new Resource<object>(obj);
                        }

                        // Add the self link.
                        resource.Links.Add(new Link
                        {
                            Rel = "self",
                            HRef = urlHelper.Action()
                        });

                        return resource;
                    };
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
            app.UseMvcWithDefaultRoute();
        }
    }
}
