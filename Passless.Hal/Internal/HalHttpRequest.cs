using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;

namespace Passless.Hal.Internal
{
    public class HalHttpRequest : HttpRequest
    {
        private IQueryFeature queryFeature;
        private IHttpRequestFeature requestFeature;
        private IRequestCookiesFeature requestCookiesFeature;
        
        public HalHttpRequest(HttpContext context)
        {
            this.HttpContext = context
                ?? throw new ArgumentNullException(nameof(context));

            this.queryFeature = context.Features.Get<IQueryFeature>()
                ?? throw new ArgumentException(
                    "context should contain a IQueryFeature",
                    nameof(context));

            this.requestFeature = context.Features.Get<IHttpRequestFeature>()
                ?? throw new ArgumentException(
                    "context should contain a IHttpRequestFeature",
                    nameof(context));

            this.requestCookiesFeature = context.Features.Get<IRequestCookiesFeature>();

        }

        public override HttpContext HttpContext { get; }

        public override string Method
        {
            get => this.requestFeature.Method;
            set => this.requestFeature.Method = value;
        }

        public override string Scheme
        {
            get => this.requestFeature.Scheme;
            set => this.requestFeature.Scheme = value;
        }

        public override bool IsHttps
        {
            get { return string.Equals("https", this.Scheme, StringComparison.OrdinalIgnoreCase); }
            set { this.Scheme = value ? "https" : "http"; }
        }

        public override HostString Host
        {
            get { return HostString.FromUriComponent(Headers["Host"]); }
            set { Headers["Host"] = value.ToUriComponent(); }
        }

        public override PathString PathBase
        {
            get { return new PathString(this.requestFeature.PathBase); }
            set { this.requestFeature.PathBase = value.Value; }
        }

        public override PathString Path
        {
            get { return new PathString(this.requestFeature.Path); }
            set { this.requestFeature.Path = value.Value; }
        }

        public override QueryString QueryString
        {
            get { return new QueryString(this.requestFeature.QueryString); }
            set { this.requestFeature.QueryString = value.Value; }
        }

        public override IQueryCollection Query
        {
            get { return this.queryFeature.Query; }
            set { this.queryFeature.Query = value; }
        }

        public override string Protocol
        {
            get { return this.requestFeature.Protocol; }
            set { this.requestFeature.Protocol = value; }
        }

        public override IHeaderDictionary Headers => this.requestFeature.Headers;

        public override IRequestCookieCollection Cookies
        {
            get => this.requestCookiesFeature?.Cookies;
            set
            {
                if (this.requestCookiesFeature != null)
                {
                    this.requestCookiesFeature.Cookies = value;
                }
            }
        }

        public override long? ContentLength
        {
            get => this.Headers.ContentLength;
            set => this.Headers.ContentLength = value;
        }

        public override string ContentType
        {
            get => this.Headers[HeaderNames.ContentType];
            set => this.Headers[HeaderNames.ContentType] = value;
        }

        public override Stream Body
        {
            get => this.requestFeature.Body;
            set => this.requestFeature.Body = value;
        }

        public override bool HasFormContentType => false;

        public override IFormCollection Form { get => null; set { } }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(default(IFormCollection));
    }
}
