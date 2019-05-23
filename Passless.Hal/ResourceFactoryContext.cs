using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal
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
