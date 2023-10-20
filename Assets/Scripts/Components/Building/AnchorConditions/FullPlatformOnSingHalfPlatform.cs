using UnityEngine;

namespace Components.Building.AnchorConditions
{
    public class FullPlatformOnSingHalfPlatform : AnchorCondition
    {
        [SerializeField] private Anchor leftAnchor;
        
        public override bool IsSatisfied(Anchor anchor)
        {
            return leftAnchor.ChildBlock != null && leftAnchor.ChildBlock.id == anchor.ParentBlock.id;
        }
    }
}