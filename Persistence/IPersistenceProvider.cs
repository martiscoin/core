using System;
using Marscore.Builder.Feature;
using Microsoft.Extensions.DependencyInjection;

namespace Marscore.Persistence
{
    /// <summary>
    /// Allow features to request a persistence implementation.
    /// </summary>
    public interface IPersistenceProvider
    {
        /// <summary>
        /// Gets the tag of the persistence implementation (usually is the name of the db engine used, e.g. "litedb" or "rocksdb").
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Gets the type of the feature for which services will be registered.
        /// </summary>
        Type FeatureType { get; }

        /// <summary>
        /// Add required services for a specific use case.
        /// </summary>
        void AddRequiredServices(IServiceCollection services);
    }
}
