using System;
namespace Passless.AspNetCore.Hal.FeatureFlags
{
    public interface IFeatureFlag
    {
        bool IsEnabled { get; set; }
    }
}
