using System;
namespace Passless.Hal.FeatureFlags
{
    public abstract class FeatureFlag : IFeatureFlag
    {
        public virtual bool IsEnabled { get; set; }
    }
}
