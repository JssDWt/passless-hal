using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Passless.Hal.Streaming
{
    public class ResourceJsonReader : WrappedJsonReader
    {
        /// <summary>
        /// Provides a concrete implementation of the abstract class <see cref="Resource" /> to populate.
        /// </summary>
        public class SerializationResource : Resource { }
        private int objectDepth = 0;
        private JsonSerializer serializer;
        private JsonContract contract;
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceJsonReader" /> class,
        /// wrapping the inner <see cref="JsonReader" /> instance.
        /// </summary>
        /// <param name="inner">The <see cref="JsonReader" /> that this one wraps around. 
        /// All methods called on this class will be invoked on the inner reader.</param>
        public ResourceJsonReader(JsonReader inner, JsonSerializer serializer)
            : this(inner, serializer, new SerializationResource())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceJsonReader" /> class,
        /// wrapping the inner <see cref="JsonReader" /> instance. The passed resource object will
        /// be populated with links and embedded resources on the fly while reading json.
        /// </summary>
        /// <param name="inner">The <see cref="JsonReader" /> that this one wraps around. 
        /// All methods called on this class will be invoked on the inner reader.</param>
        /// <param name="resource">The resource that will be populated with links and embedded resources 
        /// while reading json.</param>
        /// <remarks>It is expected that the first start object encountered is the start
        /// of the resource object to read. This reader will invoke the inner reader for anything that is not the
        /// part resource interface.</remarks>
        /// <example>TODO: Add an example of deserializing a resource instance with this class as the reader.</example>
        public ResourceJsonReader(JsonReader inner, JsonSerializer serializer, IResource resource)
            : base(inner)
        {
            this.Resource = resource
                ?? throw new ArgumentNullException(nameof(resource));

            this.serializer = serializer
                ?? throw new ArgumentNullException(nameof(serializer));

            if (resource.Links == null)
            {
                throw new ArgumentException($"Resource {nameof(IResource.Links)} cannot be null.", nameof(resource));
            }

            if (resource.Embedded == null)
            {
                throw new ArgumentException($"Resource {nameof(IResource.Embedded)} cannot be null.", nameof(resource));
            }

            this.contract = serializer.ContractResolver.ResolveContract(resource.GetType());
        }

        /// <summary>
        /// Gets the resource that is/will be populated with links and embedded resources while json is being read.
        /// </summary>
        public IResource Resource { get; }
        public bool ResourceRead { get; }

        public override bool Read()
        {
            this.ThrowIfDisposed();

            bool success = false;
            if (base.Read())
            {
                success = true;

                if (this.TokenType == JsonToken.StartObject)
                {
                    this.objectDepth++;
                }

                if (this.objectDepth == 1 && this.TokenType == JsonToken.PropertyName)
                {
                    // Currently inside the resource object and handling a property.
                    string propertyName = (string)this.Value;

                    switch (propertyName)
                    {
                        case "_links":
                            var linksType = this.Resource.Links.GetType();
                            this.Resource.Links = this.ReadRelations<ILink>(linksType);

                            // NOTE: Call this.Read(), not base.Read(), because the next property may be another reserved one. This can hardly be called recursion...
                            success = this.Read();
                            break;
                        case "_embedded":
                            var embeddedType = this.Resource.Embedded.GetType();
                            this.Resource.Embedded = this.ReadRelations<IResource>(embeddedType);

                            // NOTE: Call this.Read(), not base.Read(), because the next property may be another reserved one. This can hardly be called recursion...
                            success = this.Read();
                            break;
                        default:

                            break;
                    }
                }
            }

            return success;
        }

        private ICollection<T> ReadRelations<T>(Type type)
        {
            var relations = Activator.CreateInstance(type) as ICollection<T>;
            if (relations == null)
            {
                throw new InvalidOperationException(
                    $"Could not create an instance of type '{type.FullName}' with a default constructor.");
            }

            AdvanceToData(this.innerReader);
            int currentDepth = this.innerReader.Depth;
            while (this.innerReader.Depth >= currentDepth
                && this.innerReader.Read())
            {
                if (this.innerReader.TokenType == JsonToken.PropertyName
                    && this.innerReader.Depth == (currentDepth + 1))
                {
                    PopulateRelation(relations);
                }
            }

            return relations;
        }

        private void PopulateRelation<T>(ICollection<T> relations)
        {
            string relation = (string)this.innerReader.Value;

            // Advance to the property value.
            while (this.innerReader.TokenType != JsonToken.StartArray
                && this.innerReader.TokenType != JsonToken.StartObject
                && this.innerReader.Read())
            {
            }

            Type relationType = typeof(T);
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(relationType);

            // Set the relation based on whether it is an array or a single item.
            switch (this.innerReader.TokenType)
            {
                case JsonToken.StartArray:
                    var relationItems = (IEnumerable<T>)serializer.Deserialize(this.innerReader, enumerableType);
                    foreach (var rel in relationItems)
                    {
                        relations.Add(rel);
                    }
                    break;
                case JsonToken.StartObject:
                    var relationItem = (T)serializer.Deserialize(this.innerReader, relationType);
                    relations.Add(relationItem);
                    break;
                default:
                    throw new JsonSerializationException(
                        $"Expecting {nameof(JsonToken.StartArray)} or {nameof(JsonToken.StartObject)} token " +
                        "as the start of a relation value.");
            }
        }

        /// <summary>
        /// Advances the <see cref="JsonReader" /> to the start of an object.
        /// </summary>
        /// <param name="reader">The reader to advance.</param>
        private static void AdvanceToData(JsonReader reader)
        {
            while (reader.TokenType != JsonToken.StartObject)
            {
                if (!reader.Read())
                {
                    throw new JsonSerializationException("Expected start object.");
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (base.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
