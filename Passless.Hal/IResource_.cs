using System;
namespace Passless.Hal
{
    public interface IResource<T> : IResource where T: class
    {
        T Data { get; set; }
    }
}
