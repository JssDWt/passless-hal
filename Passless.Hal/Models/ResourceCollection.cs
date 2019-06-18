using System;
using System.Collections;
using System.Collections.Generic;

namespace Passless.Hal.Models
{
    public class ResourceCollection : Resource, IResourceCollection
    {
        private ICollection<IResource> collection = new List<IResource>();
        public ResourceCollection()
            :base()
        {
        }

        public ResourceCollection(ICollection<string> singularRelations)
            : base(singularRelations)
        {
        }

        public ResourceCollection(ICollection<ILink> links, ICollection<IResource> embedded)
            : base(links, embedded)
        {
        }

        public ResourceCollection(ICollection<string> singularRelations, ICollection<ILink> links, ICollection<IResource> embedded)
            : base(singularRelations, links, embedded)
        {
        }

        public ResourceCollection(ICollection<IResource> collection)
            : base()
        {
            this.Construct(collection);
        }

        public ResourceCollection(ICollection<IResource> collection, ICollection<string> singularRelations)
            : base(singularRelations)
        {
            this.Construct(collection);
        }

        public ResourceCollection(ICollection<IResource> collection, ICollection<ILink> links, ICollection<IResource> embedded)
            : base(links, embedded)
        {
            this.Construct(collection);
        }

        public ResourceCollection(
            ICollection<IResource> collection, 
            ICollection<string> singularRelations, 
            ICollection<ILink> links, 
            ICollection<IResource> embedded)
            : base(singularRelations, links, embedded)
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
