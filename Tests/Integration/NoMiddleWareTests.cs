using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using Passless.Hal;
using Passless.Hal.Extensions;

namespace Tests.Integration
{
    public class NoMiddleWareStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }
    }

    public class NoMiddleWareTests
    {
        private HttpClient client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .UseEnvironment("Development")
                    .ConfigureServices(services =>
                    {
                        services.AddMvcCore()
                            .AddHal();
                    })
                    .Configure(app =>
                    {
                        app.UseMvcWithDefaultRoute();
                    }));
            this.client = server.CreateClient();
        }

        [Test]
        public async Task ResourceIsSerializedTest()
        {
            var response = await this.client.GetAsync("/people/1");
            var content = response.Content.ReadAsStringAsync();
        }
    }
}
