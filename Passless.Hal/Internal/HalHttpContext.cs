using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;

namespace Passless.Hal.Internal
{
    public class HalHttpContext : HttpContext
    {
        private HttpContext context;

        public HalHttpContext(
            HttpContext context, 
            IHttpRequestFeature requestFeature)
        {
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));

            var halRequestFeature = new HalHttpRequestFeature(requestFeature);
            var halResponseFeature = new HalHttpResponseFeature();
            this.Features = new HalFeatureCollection(
                context.Features,
                halRequestFeature,
                halResponseFeature);
            this.Request = new HalHttpRequest(this);
            this.Response = new HalHttpResponse(this);
        }

        public override IFeatureCollection Features { get; }

        public override HttpRequest Request { get; }

        public override HttpResponse Response { get; }

        public override ConnectionInfo Connection => this.context.Connection;

        public override WebSocketManager WebSockets => this.context.WebSockets;

        public override AuthenticationManager Authentication => this.context.Authentication;

        public override ClaimsPrincipal User { get => this.context.User; set { } }
        public override IDictionary<object, object> Items { get => this.context.Items; set { } }
        public override IServiceProvider RequestServices { get => this.context.RequestServices; set { } }
        public override CancellationToken RequestAborted { get => this.context.RequestAborted; set { } }
        public override string TraceIdentifier { get => this.context.TraceIdentifier; set { } }
        public override ISession Session { get => this.context.Session; set { } }

        public override void Abort() { }
    }
}
