using System;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Internal
{
    public class ActionUriService : IUriService<IActionDescriptor>
    {
        private readonly ParameterParser parser;
        private readonly ValueMapper mapper;

        public ActionUriService(ParameterParser parser, ValueMapper mapper)
        {
            this.parser = parser
                ?? throw new ArgumentNullException(nameof(parser));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string GetUri(IActionDescriptor action, IUrlHelper urlHelper)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            if (action.Controller == null)
            {
                return urlHelper.Action(action.Action);
            }

            return urlHelper.Action(action.Action, action.Controller);
        }

        public string GetUri(IActionDescriptor action, IUrlHelper urlHelper, object obj)
        {
            if (obj == null)
            {
                return GetUri(action, urlHelper);
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            var parameters = parser.Parse(action.Parameter);
            var values = mapper.GetValues(parameters, obj);

            if (action.Controller == null)
            {
                return urlHelper.Action(action.Action, values);
            }

            return urlHelper.Action(action.Action, action.Controller, values);
        }
    }
}
