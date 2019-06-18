using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal.Factories
{
    public class ResourceFactoryContext
    {
        public ResourceFactoryContext()
        {
        }

        public ActionContext ActionContext { get; set; }
        public object Resource { get; set; }
        public bool IsRootResource { get; set; }
    }
}
