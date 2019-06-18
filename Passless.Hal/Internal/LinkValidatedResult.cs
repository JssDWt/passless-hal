using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal.Internal
{
    public class LinkValidatedResult : ObjectResult
    {
        public static readonly object LinkValidatedObject = "Link 'permission' confirmed(!) by link validator";
        public LinkValidatedResult() 
            : base(LinkValidatedObject)
        {
        }
    }
}
