using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Models;

namespace Passless.AspNetCore.Hal.Converters
{
    public class ResourceJsonConverter : JsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceJsonConverter" /> class.
        /// </summary>
        public ResourceJsonConverter()
        {

        }

        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override bool CanConvert(Type objectType)
        {
            bool result = false;
            if (objectType != null)
            {
                result = typeof(IResource).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
            }

            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
            //IResource result = null;

            //if (!this.CanConvert(objectType))
            //{
            //    throw new ArgumentException(
            //        $"Cannot read as type '{objectType?.FullName ?? "[NULL]"}', because it is not a '{typeof(IResource).FullName}'");
            //}

            //if (reader == null) throw new ArgumentNullException(nameof(reader));
            //if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            //var jObject = JObject.Load(reader);

            //if (jObject.TryGetValue(Resource.LinksPropertyName, out JToken linksToken))
            //{
            //    jObject.Remove(Resource.LinksPropertyName);
            //}

            //if (jObject.TryGetValue(Resource.EmbeddedPropertyName, out JToken embeddedToken))
            //{
            //    jObject.Remove(Resource.EmbeddedPropertyName);
            //}

            //if (objectType.IsOfGenericType(typeof(Resource<>)))
            //{
            //    var valueTypes = objectType.GetGenericArguments(typeof(Resource<>));
            //    if (valueTypes == null
            //        || valueTypes.Length != 1)
            //    {
            //        throw new ArgumentException(
            //            "The specified object type does not meet the constraints needed to deserialize it as a Resource.");
            //    }

            //    var valueType = valueTypes.Single();

            //    var objectContract = serializer.ContractResolver.ResolveContract(objectType);
            //    result = (IResource)objectContract.DefaultCreator();
            //    var dataProperty = typeof(Resource<>).MakeGenericType(valueTypes).GetProperty(nameof(Resource<dynamic>.Data));
            //    var data = jObject.ToObject(valueType, serializer);
            //    dataProperty.SetValue(result, data);
            //}
            //else
            //{
            //    try
            //    {
            //        this.canRead = false;
            //        result = (IResource)jObject.ToObject(objectType, serializer);
            //    }
            //    finally
            //    {
            //        this.canRead = true;
            //    }
                
            //}

            //if (result != null)
            //{
            //    if (linksToken != null)
            //    {
            //        result.Links = ReadRelations<ILink>(result.Links.GetType(), (JObject)linksToken, serializer);
            //    }

            //    if (embeddedToken != null)
            //    {
            //        result.Embedded = ReadRelations<IResource>(result.Embedded.GetType(), (JObject)embeddedToken, serializer);
            //    }
            //}

            //return result;
        }

        //private ICollection<T> ReadRelations<T>(Type type, JObject jObject, JsonSerializer serializer) where T : IRelated
        //{
        //    var relations = Activator.CreateInstance(type) as ICollection<T>;
        //    if (relations == null)
        //    {
        //        throw new InvalidOperationException(
        //            $"Could not create an instance of type '{type.FullName}' with a default constructor.");
        //    }

        //    foreach(var property in jObject)
        //    {
        //        JToken token = property.Value;
        //        switch (token.Type)
        //        {
        //            case JTokenType.Array:
        //                var items = token.ToObject<List<T>>(serializer);
        //                foreach (var item in items)
        //                {
        //                    item.Rel = property.Key;
        //                    relations.Add(item);
        //                }
        //                break;

        //            case JTokenType.Object:
        //                var current = token.ToObject<T>(serializer);
        //                current.Rel = property.Key;
        //                relations.Add(current);
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return relations;
        //}

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) return;

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (!(value is IResource resource))
            {
                throw new ArgumentException(
                    $"Cannot convert type {value.GetType().FullName}, because it is not an {typeof(IResource).FullName}.", 
                    nameof(value));
            }

            object data = null;
            Type dataType = null;
            JsonObjectContract dataContract = null;
            var resourceType = resource.GetType();
            var resourceContract = serializer.ContractResolver.ResolveContract(resourceType) as JsonObjectContract;

            if (resourceContract == null)
            {
                throw new JsonSerializationException($"Could not resolve object contract for type '{resourceType}'.");
            }

            if (resourceType.IsOfGenericType(typeof(Resource<>)))
            {
                // Data property of Resource<> cannot be null, so data is now never null.
                var dataProperty = resourceType.GetProperty(nameof(Resource<dynamic>.Data));
                data = dataProperty.GetValue(resource);
                dataType = data?.GetType();

                if (data != null)
                {
                    // TODO: Throw if the datatype is a collection.
                    dataContract = serializer.ContractResolver.ResolveContract(dataType) as JsonObjectContract;
                }
            }
            else
            {
                data = resource;
                dataType = resourceType;
                dataContract = resourceContract;
            }

            writer.WriteStartObject();

            if (resourceContract.Properties[Resource.LinksPropertyName]?.ShouldSerialize(resource) == true)
            {
                WriteLinks(serializer, writer, resource);
            }

            if (resourceContract.Properties[Resource.EmbeddedPropertyName]?.ShouldSerialize(resource) == true
                || (resource is IResourceCollection collection && collection.Collection?.Count > 0))
            {
                WriteEmbedded(serializer, writer, resource);
            }

            if (data != null)
            {
                if (dataContract == null)
                {
                    throw new JsonSerializationException("Could not resolve contract for the HAL resource.");
                }

                foreach (var property in dataContract.Properties.Where(p => !Resource.ReservedProperties.Contains(p.PropertyName))
                    .Where(p => !p.Ignored && (p.ShouldSerialize == null || p.ShouldSerialize(data))))
                {
                    writer.WritePropertyName(property.PropertyName);
                    var propertyValue = dataType.GetRuntimeProperty(property.UnderlyingName).GetValue(data);
                    serializer.Serialize(writer, propertyValue);
                }
            }

            writer.WriteEndObject();
        }

        //private void WriteRelations<T>(
        //   JsonSerializer serializer,
        //   JsonWriter writer,
        //   ICollection<T> relations,
        //   ICollection<string> singularRelations) where T : IRelated
        //{
        //    writer.WriteStartObject();
        //    foreach (var relation in relations.GroupBy(r => r.Rel))
        //    {
        //        writer.WritePropertyName(relation.Key);

        //        var relationItems = relation.ToList();
        //        if (singularRelations.Contains(relation.Key))
        //        {
        //            if (relationItems.Count > 1)
        //            {

        //            }

        //            serializer.Serialize(writer, relationItems.First());
        //        }
        //        else
        //        {
        //            serializer.Serialize(writer, relationItems);
        //        }
        //    }

        //    writer.WriteEndObject();
        //}

        private void WriteLinks(JsonSerializer serializer, JsonWriter writer, IResource resource)
        {
            writer.WritePropertyName(Resource.LinksPropertyName);
            writer.WriteStartObject();
            foreach (var relation in resource.Links.GroupBy(l => l.Rel))
            {
                WriteRelation(serializer, writer, relation.Key, relation.ToList(), resource.SingularRelations);
            }
            writer.WriteEndObject();
        }

        private void WriteEmbedded(JsonSerializer serializer, JsonWriter writer, IResource resource)
        {
            writer.WritePropertyName(Resource.EmbeddedPropertyName);
            writer.WriteStartObject();
            bool isCollection = false;
            string embedRel = null;
            ICollection<IResource> embedItems = null;
            bool collectionProcessed = false;
            if (resource is IResourceCollection collection)
            {
                embedRel = collection.EmbedRel;
                embedItems = collection.Collection;
                isCollection = embedItems != null;
            }

            foreach (var relation in resource.Embedded.GroupBy(l => l.Rel))
            {
                var rel = relation.Key;
                var items = relation.ToList();

                if (isCollection && rel == embedRel)
                {
                    items.AddRange(embedItems);
                    collectionProcessed = true;
                }

                WriteRelation(serializer, writer, rel, items, resource.SingularRelations);
            }

            if (isCollection && !collectionProcessed)
            {
                WriteRelation(serializer, writer, embedRel, embedItems, resource.SingularRelations);
            }

            writer.WriteEndObject();
        }

        private void WriteRelation<T>(JsonSerializer serializer, JsonWriter writer, string rel, ICollection<T> items, ICollection<string> singularRelations)
        {
            bool isSingular = singularRelations.Contains(rel);
            if (isSingular && items.Count > 1)
            {
                throw new JsonSerializationException(
                    $"Could not serialize resource, because the relation '{rel}' is marked singular, but contains {items.Count} items.");
            }

            writer.WritePropertyName(rel);

            if (isSingular)
            {
                serializer.Serialize(writer, items.First());
            }
            else
            {
                serializer.Serialize(writer, items);
            }
        }
    }

   
}
