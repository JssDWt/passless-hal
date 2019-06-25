using System;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Internal
{
    public class RouteUriService : IUriService<IRouteDescriptor>
    {
        private readonly ParameterParser parser;
        private readonly ValueMapper mapper;

        public RouteUriService(ParameterParser parser, ValueMapper mapper)
        {
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string GetUri(IRouteDescriptor route, IUrlHelper urlHelper)
            => GetUri(route, urlHelper, null);

        public string GetUri(IRouteDescriptor route, IUrlHelper urlHelper, object obj)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            if (obj == null)
            {
                return urlHelper.RouteUrl(route.RouteName);
            }

            var parameters = parser.Parse(route.Parameter);
            var values = mapper.GetValues(parameters, obj);

            return urlHelper.RouteUrl(route.RouteName, values);
        }
    }
}
