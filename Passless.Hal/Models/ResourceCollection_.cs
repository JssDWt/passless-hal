using System;
using System.Collections.Generic;

namespace Passless.AspNetCore.Hal.Models
{
    public class ResourceCollection<T> : Resource<T>, IResourceCollection<T> where T : class
    {
        private ICollection<IResource> collection = new List<IResource>();

        public ResourceCollection()
            : base()
        {

        }

        public ResourceCollection(T data)
            : base(data)
        {

        }

        public ResourceCollection(T data, ICollection<string> singularRelations)
            : base(data, singularRelations)
        {

        }

        public ResourceCollection(T data, ICollection<ILink> links, ICollection<IResource> embedded)
            : base(data, links, embedded)
        {

        }

        public ResourceCollection(T data, ICollection<string> singularRelations, ICollection<ILink> links, ICollection<IResource> embedded)
            : base(data, singularRelations, links, embedded)
        {

        }

        public ResourceCollection(ICollection<IResource> collection)
            : base()
        {
            this.Construct(collection);
        }

        public ResourceCollection(T data, ICollection<IResource> collection)
            : base(data)
        {
            this.Construct(collection);
        }

        public ResourceCollection(T data, ICollection<IResource> collection, ICollection<string> singularRelations)
            : base(data, singularRelations)
        {
            this.Construct(collection);
        }

        public ResourceCollection(T data, ICollection<IResource> collection, ICollection<ILink> links, ICollection<IResource> embedded)
            : base(data, links, embedded)
        {
            this.Construct(collection);
        }

        public ResourceCollection(T data, ICollection<IResource> collection, ICollection<string> singularRelations, ICollection<ILink> links, ICollection<IResource> embedded)
            : base(data, singularRelations, links, embedded)
        {
            this.Construct(collection);
        }

        public string EmbedRel { get; set; } = "items";

        public ICollection<IResource> Collection
        {
            get => this.collection;
            set => this.collection = value
                ?? throw new ArgumentNullException(nameof(Collection));
        }

        private void Construct(ICollection<IResource> collection)
        {
            this.collection = collection 
                ?? throw new ArgumentNullException(nameof(collection));
        }
    }
}
