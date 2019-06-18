using System;
using System.Collections;
using Passless.Hal.Models;

namespace Passless.Hal.Factories
{
    public class DefaultHalResourceFactory : IHalResourceFactory
    {
        public IResource CreateResource(ResourceFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var resource = this.CreateResource(context.Resource);
            return resource;
        }

        private IResource CreateResource(object obj)
        {
            if (obj is IResource resource)
            {
                return resource;
            }

            if (obj is IEnumerable enumerable)
            {
                var collection = new ResourceCollection<object>();
                foreach (var item in enumerable)
                {
                    var inner = this.CreateResource(item);
                    collection.Collection.Add(inner);
                }

                return collection;
            }

            return new Resource<object>(obj);
        }
    }
}
