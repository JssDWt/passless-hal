using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Passless.AspNetCore.Hal.Internal
{
    public class HalHttpResponseFeature : IHttpResponseFeature
    {
        public HalHttpResponseFeature()
        {
        }

        public int StatusCode { get; set; } = 200;
        public string ReasonPhrase { get; set; }
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public Stream Body { get; set; } = Stream.Null;

        public bool HasStarted => false;

        public void OnCompleted(Func<object, Task> callback, object state)
        { 
        }

        public void OnStarting(Func<object, Task> callback, object state)
        {

        }
    }
}
