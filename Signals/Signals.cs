using Marscore.EventBus;
using Microsoft.Extensions.Logging;

namespace Marscore.Signals
{
    public interface ISignals : IEventBus
    {
    }

    public class Signals : InMemoryEventBus, ISignals
    {
        public Signals(ILoggerFactory loggerFactory, ISubscriptionErrorHandler subscriptionErrorHandler) : base(loggerFactory, subscriptionErrorHandler) { }
    }
}
