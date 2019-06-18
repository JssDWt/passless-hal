using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal.Inspectors
{
    public class HalResourceInspectedContext
    {

        public HalResourceInspectedContext(HalResourceInspectingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.Resource = context.Resource;
            this.ActionContext = context.ActionContext;
            this.IsRootResource = context.IsRootResource;
        }

        /// <summary>
        /// Gets or sets the resource that is currently being inspected.
        /// </summary>
        /// <remarks>Note that the resource may be <c>null</c>.</remarks>
        public IResource Resource { get; }

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
    }
}
