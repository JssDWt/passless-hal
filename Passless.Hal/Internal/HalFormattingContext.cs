using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Passless.Hal.Internal
{
    public class HalFormattingContext
    {
        public ActionContext Context { get; set; }
        public ObjectResult Result { get; set; }
        public IActionResultExecutor<ObjectResult> Executor { get; set; }
    }
}
