using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components.Building.AnchorConditions
{
    public class FullPlatformOnSingleHalfPlatform : AnchorCondition
    {
        [SerializeField] private Anchor rightAnchor;
        private readonly char[] _side = {'U', 'R', 'D', 'L'};
        
        public override bool IsSatisfied(Anchor anchor)
        {
            bool result = rightAnchor.ChildBlock != null && rightAnchor.ChildBlock.type == BlockType.HalfPlatform;

            if (!result)
            {
                List<Block> blocks = new List<Block>();
                foreach (var a in anchor.ParentBlock.Anchors)
                {
                    if (a.ParentBlock != null && 
                        !blocks.Contains(a.ParentBlock) &&
                        a.ParentBlock.type == BlockType.Platform)
                    {
                        blocks.Add(a.ParentBlock);
                    }
                }
                
                string selfDirection = System.Text.RegularExpressions.Regex.Match(anchor.GetSelfPath(), @"[URDL]").Value;
                string wantedDirection = selfDirection switch
                {
                    "U" => "L",
                    "R" => "U",
                    "D" => "R",
                    "L" => "D",
                    _ => "U"
                };

                if (anchor.ParentBlock.type is BlockType.HalfPlatform)
                {
                    foreach (var b in blocks)
                    {
                        if (!b.ChildrenDictionary.ContainsKey("HalfPlatform_" + wantedDirection + "_1")) continue;
                        if (b.ChildrenDictionary["HalfPlatform_" + wantedDirection + "_1"].ChildBlock == null) continue;
                        if (b.ChildrenDictionary["HalfPlatform_" + wantedDirection + "_1"].ChildBlock !=
                            anchor.ParentBlock) continue;

                        result = true;
                    }
                }
                else if (anchor.ParentBlock.type is BlockType.Platform)
                {
                    foreach (var b in blocks)
                    {
                        if (!b.ChildrenDictionary.ContainsKey("Platform_" + wantedDirection + "_0")) continue;
                        if (b.ChildrenDictionary["Platform_" + wantedDirection + "_0"].ChildBlock == null) continue;
                        if (b.ChildrenDictionary["Platform_" + wantedDirection + "_0"].ChildBlock !=
                            anchor.ParentBlock) continue;

                        result = true;
                    }
                }
            }
            
            return result;
        }
    }
}