using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Passless.Hal.Internal
{
    public class HalHttpResponse : HttpResponse
    {
        private IHttpResponseFeature responseFeature;
        public HalHttpResponse(HttpContext context)
        {
            this.HttpContext = context
                ?? throw new ArgumentNullException(nameof(context));

            this.responseFeature = context.Features.Get<IHttpResponseFeature>()
                ?? throw new ArgumentException(
                    "context should contain a IHttpResponseFeature",
                    nameof(context));
        }

        public object Resource { get; set; }
        public ActionContext ActionContext { get; set; }

        public override HttpContext HttpContext { get; }

        public override int StatusCode
        {
            get => this.responseFeature.StatusCode;
            set => this.responseFeature.StatusCode = value;
        }

        public override IHeaderDictionary Headers => this.responseFeature.Headers;

        public override Stream Body
        {
            get => this.responseFeature.Body;
            set => this.responseFeature.Body = value;
        }

        public override long? ContentLength
        {
            get => this.Headers.ContentLength;
            set => this.Headers.ContentLength = value;
        }

        public override string ContentType
        {
            get
            {
                return Headers[HeaderNames.ContentType];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.responseFeature.Headers.Remove(HeaderNames.ContentType);
                }
                else
                {
                    this.responseFeature.Headers[HeaderNames.ContentType] = value;
                }
            }
        }

        public override IResponseCookies Cookies => null;

        public override bool HasStarted => this.responseFeature.HasStarted;

        public override void OnCompleted(Func<object, Task> callback, object state)
            => this.responseFeature.OnCompleted(callback, state);

        public override void OnStarting(Func<object, Task> callback, object state)
            => this.responseFeature.OnStarting(callback, state);

        public override void Redirect(string location, bool permanent)
        {
            if (permanent)
            {
                this.responseFeature.StatusCode = 301;
            }
            else
            {
                this.responseFeature.StatusCode = 302;
            }

            Headers[HeaderNames.Location] = location;
        }
    }
}
