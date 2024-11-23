using Blockcore.Configuration;

namespace Blockcore.Node
{
    public static class NetworkSelector
    {
        public static NodeSettings Create(string chain, string[] args)
        {
            chain = chain.ToUpperInvariant();

            NodeSettings nodeSettings = null;

            switch (chain)
            {
                case "X1":
                    nodeSettings = new NodeSettings(networksSelector: Blockcore.Networks.X1.Networks.X1, args: args);
                    break;
            }

            return nodeSettings;
        }
    }
}