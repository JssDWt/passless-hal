using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Passless.Hal
{
    /// <summary>
    /// Generic class describing a resource. This class can be used instead of an inheritance stategy for HAL resources.
    /// </summary>
    public class Resource<T> : Resource, IResource<T> where T: class
    {
        /// <summary>
        /// Backing field for the Data property.
        /// </summary>
        private T data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource{T}"/> class.
        /// </summary>
        public Resource()
            : base()
        {
            // TODO: Remove this constructor?
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class with the specified data.
        /// </summary>
        /// <param name="data">The data to use as the body of this resource.</param>
        public Resource(T data)
            : base()
        {
            this.Construct(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class with the specified data,
        /// using the specified data and singular relations.
        /// </summary>
        /// <param name="data">The data to use as the body of this resource.</param>
        /// <param name="singularRelations">Singular relations.</param>
        public Resource(T data, ICollection<string> singularRelations)
            : base(singularRelations)
        {
            this.Construct(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class with the specified data,
        /// using the specified links and embedded resources.
        /// </summary>
        /// <param name="data">The data to use as the body of this resource.</param>
        /// <param name="links">The links for the current resource.</param>
        /// <param name="embedded">The embedded resources for the current resource.</param>
        public Resource(
            T data,
            ICollection<ILink> links,
            ICollection<IResource> embedded)
            : base(links, embedded)
        {
            this.Construct(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class with the specified data,
        /// using the specified links, embedded resources and singular relations.
        /// </summary>
        /// <param name="data">The data to use as the body of this resource.</param>
        /// <param name="singularRelations">Singular relations.</param>
        /// <param name="links">The links for the current resource.</param>
        /// <param name="embedded">The embedded resources for the current resource.</param>
        public Resource(
            T data,
            ICollection<string> singularRelations,
            ICollection<ILink> links,
            ICollection<IResource> embedded)
            : base(singularRelations, links, embedded)
        {
            this.Construct(data);
        }

        /// <summary>
        /// Gets or sets the data of the current resource.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the data is set to null.</exception>
        /// <exception cref="ArgumentException">Thrown when the data object implements <see cref="IResource" />.</exception>
        [JsonIgnore]
        public T Data
        {
            get => this.data;
            set
            {
                if (value is IResource)
                {
                    throw new ArgumentException(
                        $"Cannot set value to an object that implements the {nameof(IResource)} interface. " +
                        "That would break the HAL constraints.", nameof(Data));
                }

                this.data = value;
            }
        }

        /// <summary>
        /// Constructs the current object with the specified data.
        /// Used because the constructors cannot be chained.
        /// </summary>
        /// <param name="data">The data to use as the data for the current resource.</param>
        private void Construct(T data)
        {
            // NOTE: Argument validation is done in the Data property.
            this.Data = data;
        }
    }
}
