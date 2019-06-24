using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Inspectors;

namespace Tests
{
    public class ResourcePipelineInvokerTests
    {
        // TODO: Test resourcefactory is invoked.
        // TODO: Test resourcefactory is invoked for subresources as well.
        // TODO: Test only root resourceinspectors are invoked for the root resource.
        // TODO: Test only embed resourceinspectors are invoked for the embedded resources.

        
    }
}
