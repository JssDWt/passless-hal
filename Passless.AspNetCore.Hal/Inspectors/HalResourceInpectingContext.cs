using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Inspectors
{
    /// <summary>
    /// Hal resource inspecting context. Used to inspect hal resources.
    /// It is recommended to use this context for adding links to resources.
    /// </summary>
    public class HalResourceInspectingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalResourceInspectingContext"/> class.
        /// </summary>
        /// <param name="resource">Resource.</param>
        /// <param name="actionContext">Action context.</param>
        /// <param name="isRootResource">If set to <c>true</c> is root resource.</param>
        public HalResourceInspectingContext(
            IResource resource,
            ActionContext actionContext,
            bool isRootResource,
            ResourceFactory resourceFactory,
            MvcPipeline mvcPipeline,
            object original)
        {
            this.Resource = resource
                ?? throw new ArgumentNullException(nameof(resource));

            this.ActionContext = actionContext
                ?? throw new ArgumentNullException(nameof(actionContext));

            this.IsRootResource = isRootResource;

            this.ResourceFactory = resourceFactory
                ?? throw new ArgumentNullException(nameof(resourceFactory));

            this.MvcPipeline = mvcPipeline
                ?? throw new ArgumentNullException(nameof(mvcPipeline));

            this.OriginalObject = original
                ?? throw new ArgumentNullException(nameof(original));
        }

        /// <summary>
        /// Gets or sets the resource that is currently being inspected.
        /// </summary>
        /// <remarks>Note that the resource may be <c>null</c>.</remarks>
        public IResource Resource { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Resource"/> is the root resource
        /// of the HAL document.
        /// </summary>
        /// <value><c>true</c> if is root resource; otherwise, <c>false</c>.</value>
        public bool IsRootResource { get; }

        /// <summary>
        /// Gets the action context that this resource came from.
        /// </summary>
        /// <value>The action context that created the resource.</value>
        public ActionContext ActionContext { get; }

        /// <summary>
        /// Gets the resource factory for creating embedded resources.
        /// </summary>
        /// <value>The resource factory.</value>
        /// <remarks>It is recommended to use the resource factory to create embedded resources,
        /// instead of creating an IResource object yourself. In order to be able to use the 
        /// resource inspectors for embedded resources as well.</remarks>
        public ResourceFactory ResourceFactory { get; }

        public MvcPipeline MvcPipeline { get; }

        public object OriginalObject { get; }
    }
}
