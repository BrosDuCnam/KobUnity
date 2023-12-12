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
            Vector3 localPosition = parent.build.transform.InverseTransformPoint(child.transform.position);
            BuildNode.Instantiate(child.type, parent.transform.parent, localPosition, parent.build, id);
        }
    }
}