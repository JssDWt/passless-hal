using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal.Factories;

namespace Passless.Hal.Extensions
{
    public class HalOptions
    {
        private ICollection<string> supportedMediaTypes = new List<string>
        {
            "application/hal+json"
        };

        private ICollection<IHalResourceFactoryMetadata> resourceFactories
            = new List<IHalResourceFactoryMetadata>();

        public ICollection<string> SupportedMediaTypes
        {
            get => this.supportedMediaTypes;
            set => this.supportedMediaTypes = value
                ?? throw new ArgumentNullException(nameof(SupportedMediaTypes));
        }

        public ICollection<IHalResourceFactoryMetadata> ResourceFactories 
        {
            get => this.resourceFactories;
            set => this.resourceFactories = value
                ?? throw new ArgumentNullException(nameof(ResourceFactories));
        }
            
    }
}
