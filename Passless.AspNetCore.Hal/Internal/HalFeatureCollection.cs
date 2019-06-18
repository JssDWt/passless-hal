using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;

namespace Passless.AspNetCore.Hal.Internal
{
    public class HalFeatureCollection : IFeatureCollection
    {
        private int revision = 0;
        private Dictionary<Type, object> features = new Dictionary<Type, object>();

        public HalFeatureCollection(
            IFeatureCollection features, 
            IHttpRequestFeature requestFeature,
            IHttpResponseFeature responseFeature)
        {
            this.features.Add(typeof(IHttpRequestFeature), requestFeature);
            this.features.Add(typeof(IHttpResponseFeature), responseFeature);

            foreach (var feature in features)
            {
                if (feature.Key.IsAssignableFrom(typeof(IHttpRequestFeature))
                    || feature.Key.IsAssignableFrom(typeof(IHttpResponseFeature))
                    || feature.Key.IsAssignableFrom(typeof(IQueryFeature))
                    || feature.Key.IsAssignableFrom(typeof(IFormFeature))
                    || feature.Key.IsAssignableFrom(typeof(IResponseCookiesFeature)))
                {
                    continue;
                }

                this.features[feature.Key] = feature.Value;
            }

            this.features.Add(typeof(IQueryFeature), new QueryFeature(this));
        }

        public object this[Type key] 
        { 
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                this.features.TryGetValue(key, out object feature);
                return feature;
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (value == null)
                {
                    if (this.features.Remove(key))
                    {
                        this.revision++;
                    }

                    return;
                }

                this.features[key] = value;
                this.revision++;
            }
        }

        public bool IsReadOnly => true;

        public int Revision => this.revision;

        public TFeature Get<TFeature>() => (TFeature)this[typeof(TFeature)];
        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
        {
            foreach (var pair in this.features)
            {
                yield return pair;
            }
        }

        public void Set<TFeature>(TFeature instance) => this[typeof(TFeature)] = instance;
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
