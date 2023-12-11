using UnityEngine;

namespace Components.Building
{
    public class BuildAnchor : MonoBehaviour
    {
        [SerializeField] public BuildNode parent;
        [SerializeField] public BuildNode child;

        public void Build(int id)
        {
            BuildNode.Instantiate(child.type, parent.transform.parent, child.transform.position, parent.build, id);
        }
    }
}