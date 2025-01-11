using System;
using Martiscoin.Builder.Feature;
using Martiscoin.Configuration;
using Martiscoin.Networks;
using Martiscoin.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Martiscoin.Builder
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
