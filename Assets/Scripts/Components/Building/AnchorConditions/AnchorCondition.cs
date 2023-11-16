using UnityEngine;

namespace Components.Building.AnchorConditions
{
    public abstract class AnchorCondition : MonoBehaviour
    {
        public abstract bool IsSatisfied(Anchor anchor);
    }
}