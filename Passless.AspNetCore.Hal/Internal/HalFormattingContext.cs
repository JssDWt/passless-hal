using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Passless.AspNetCore.Hal.Internal
{
    public class HalFormattingContext
    {
        public HalFormattingContext(ActionContext context, ObjectResult result, IActionResultExecutor<ObjectResult> executor)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Result = result ?? throw new ArgumentNullException(nameof(result));
            this.Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public ActionContext Context { get; }
        public ObjectResult Result { get; }
        public IActionResultExecutor<ObjectResult> Executor { get; }
    }
}
