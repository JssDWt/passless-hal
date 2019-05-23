using System;
using System.Collections.Generic;

namespace Passless.Hal
{
    public interface IResourceCollection<T> : IResource<T>, IResourceCollection where T: class
    {
    }
}
