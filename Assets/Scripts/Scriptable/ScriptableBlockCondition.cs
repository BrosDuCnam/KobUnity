using Components.Building;

namespace Scriptable
{
    public abstract class ScriptableBlockCondition : ScriptableItem
    {
        public abstract bool Check(Block block, Anchor anchor);
    }
}