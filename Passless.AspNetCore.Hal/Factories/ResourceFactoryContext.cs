using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Factories
{
    public class ResourceFactoryContext
    {
        public ActionContext ActionContext { get; set; }
        public object Resource { get; set; }
        public bool IsRootResource { get; set; }
    }
}
