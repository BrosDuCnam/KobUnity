using Components.Data;
using Unity.Netcode;

namespace Network.Data
{
    public class NetworkBuild : NetworkData<GraphNodeBuild>
    {
        private NetworkList<BlockNode> _items = null;

        private void Awake()
        {
            _items = new NetworkList<BlockNode>();
        }
        
        public override GraphNodeBuild GetValue()
        {
            GraphNodeBuild build = new GraphNodeBuild();
            foreach (var item in _items)
            {
                build.AddNode(item);
            }
            
            return build;
        }
    }
}