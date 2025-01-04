using System;
using Marscore.Builder.Feature;
using Marscore.Configuration;
using Marscore.Networks;
using Marscore.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Marscore.Builder
{
    /// <summary>
    /// Allow specific network implementation to override services.
    /// </summary>
    public interface IFullNodeBuilderServiceOverride
    {
        /// <summary>
        /// Intercept the builder to override services.
        /// </summary>
        void OverrideServices(IFullNodeBuilder builder);
    }
}
