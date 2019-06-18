using System;
namespace Passless.Hal.FeatureFlags
{
    public interface IFeatureFlag
    {
        bool IsEnabled { get; set; }
    }
}
