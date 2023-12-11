using System.Linq;
using UnityEngine;

namespace Components.Building
{
    public class BuildAnchor : MonoBehaviour
    {
        [SerializeField] public BuildNode parent;
        [SerializeField] public BuildNode child;
        
        public int AnchorId => parent.anchors.FirstOrDefault(x => x.Value == this).Key;
        public bool IsAvailable => !parent.children.TryGetValue(AnchorId, out BuildNode value) || value == null;

        public void Build(int id)
        {
            BuildNode.Instantiate(child.type, parent.transform.parent, child.transform.position, parent.build, id);
        }
    }
}