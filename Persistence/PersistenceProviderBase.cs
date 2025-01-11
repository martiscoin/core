using System;
using Martiscoin.Builder.Feature;
using Microsoft.Extensions.DependencyInjection;

namespace Martiscoin.Persistence
{
    /// <inheritdoc/>
    public abstract class PersistenceProviderBase<TFeature> : IPersistenceProvider where TFeature : IFullNodeFeature
    {
        /// <inheritdoc/>
        public abstract string Tag { get; }

        /// <inheritdoc/>
        public Type FeatureType => typeof(TFeature);

        /// <inheritdoc/>
        public abstract void AddRequiredServices(IServiceCollection services);
    }
}
