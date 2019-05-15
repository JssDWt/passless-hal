using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal.Extensions
{
    public class HalOptions
    {
        private ICollection<string> supportedMediaTypes = new List<string>
        {
            "application/hal+json"
        };

        public ICollection<string> SupportedMediaTypes
        {
            get => this.supportedMediaTypes;
            set
            {
                this.supportedMediaTypes = value
                    ?? throw new ArgumentNullException(nameof(SupportedMediaTypes));
            }
        }

        public Func<object, ActionContext, IUrlHelper, Task<object>> ResourceFactory { get; set; }
    }
}
