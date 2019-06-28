using System;
using System.Collections.Generic;
using Passless.AspNetCore.Hal.Inspectors;

namespace Passless.AspNetCore.Hal.Extensions
{
    /// <summary>
    /// Options for modifying HAL behavior.
    /// </summary>
    public class HalOptions
    {
        private ICollection<string> supportedMediaTypes = new List<string>
        {
            "application/hal+json"
        };

        /// <summary>
        /// Gets or sets the supported HAL mediatypes.
        /// </summary>
        /// <remarks>By default, this contains 'application/hal+json'.</remarks>
        public ICollection<string> SupportedMediaTypes
        {
            get => this.supportedMediaTypes;
            set => this.supportedMediaTypes = value
                ?? throw new ArgumentNullException(nameof(SupportedMediaTypes));
        }

        private IList<IHalResourceInspectorMetadata> resourceInspectors
            = new List<IHalResourceInspectorMetadata>();

        /// <summary>
        /// Gets or sets the resource inspectors to inspect and modify returned resources.
        /// </summary>
        /// <remarks>In order to add and remove resource inspecors, create an
        /// <see cref="Microsoft.Extensions.Options.IConfigureOptions{HalOptions}"/>
        /// to add inspectors that have dependencies from the DI container.
        /// Use <see cref="Microsoft.Extensions.Options.IPostConfigureOptions{HalOptions}"/>
        /// to make sure the order of the inspectors is correct.</remarks>
        public IList<IHalResourceInspectorMetadata> ResourceInspectors
        {
            get => this.resourceInspectors;
            set => this.resourceInspectors = value
                ?? throw new ArgumentNullException(nameof(ResourceInspectors));
        }

        /// <summary>
        /// Value indicating whether the default HAL resource inspectors should be added to the pipeline.
        /// </summary>
        /// <remarks>
        /// This includes inspectors for attribute embedding/linking, resource validation and link permission.
        /// </remarks>
        public bool UseDefaultResourceInspectors { get; set; } = true;
    }
}
