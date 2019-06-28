using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;

namespace Passless.AspNetCore.Hal.Extensions
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
            set => this.supportedMediaTypes = value
                ?? throw new ArgumentNullException(nameof(SupportedMediaTypes));
        }

        public IList<IHalResourceInspectorMetadata> ResourceInspectors { get; set; }
            = new List<IHalResourceInspectorMetadata>();

        public bool UseDefaultResourceInspectors { get; set; } = true;
    }
}
