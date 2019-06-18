using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal.Factories;
using Passless.Hal.Inspectors;

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
            set => this.supportedMediaTypes = value
                ?? throw new ArgumentNullException(nameof(SupportedMediaTypes));
        }

        private IHalResourceFactoryMetadata resourceFactory;
        public IHalResourceFactoryMetadata ResourceFactory 
        {
            get => this.resourceFactory;
            set => this.resourceFactory = value
                ?? throw new ArgumentNullException(nameof(ResourceFactory));
        }

        public IList<IHalResourceInspectorMetadata> ResourceInspectors { get; set; }
            = new List<IHalResourceInspectorMetadata>();

        public bool UseDefaultResourceFactory { get; set; } = true;

        public bool UseDefaultResourceInspectors { get; set; } = true;
    }
}
