using System;
namespace Passless.AspNetCore.Hal.FeatureFlags
{
    public abstract class FeatureFlag : IFeatureFlag
    {
        public virtual bool IsEnabled { get; set; }
    }
}
