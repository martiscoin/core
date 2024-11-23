using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blockcore.Builder;
using Blockcore.Configuration;
using Blockcore.Networks.X1;
using Blockcore.Utilities;

namespace Blockcore.Node
{
    public class Program
    {
        static IFullNode node;

        public static async Task Main(string[] args)
        {
            try
            {
                string chain = args
                   .DefaultIfEmpty("--chain=BTC")
                   .Where(arg => arg.StartsWith("--chain", ignoreCase: true, CultureInfo.InvariantCulture))
                   .Select(arg => arg.Replace("--chain=", string.Empty, ignoreCase: true, CultureInfo.InvariantCulture))
                   .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(chain))
                {
                    chain = "BTC";
                }

                NodeSettings nodeSettings = NetworkSelector.Create(chain, args);
                IFullNodeBuilder nodeBuilder = NodeBuilder.Create(chain, nodeSettings);

                node = nodeBuilder.Build();
                ((X1Main)nodeSettings.Network).Parent = node;

                if (node != null)
                    await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex);
            }
        }

        public static void Shutdown()
        {
            node?.NodeLifetime.StopApplication();
        }
    }
}
