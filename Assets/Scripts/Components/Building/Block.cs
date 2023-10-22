using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Components.Building
{
    public enum BlockType
    {
        None,
        HalfPlatform,
        Platform,
    }
    
    public class Block : MonoBehaviour
    {
        [field: SerializeField] public int id { get; private set; }
        [field: SerializeField] public BlockType type { get; private set; }

        [Space] 
        
        [SerializeField] private List<Anchor> anchors = new (); // fallback attach points
        
        [Header("References")]
        [SerializeField] private SerializedDictionary<string, Anchor> childrenAnchor = new (); // anchor, name
        [SerializeField] private List<Collider> colliders = new ();

        public List<Anchor> Anchors => anchors;
        
        public List<Anchor> ChildAnchors => childrenAnchor.Values.ToList();
        public SerializedDictionary<string, Anchor> ChildrenDictionary => childrenAnchor;
        
        
        public void OnValidate()
        {
            if (id == 0)
            {
                id = Guid.NewGuid().GetHashCode();
            }
        }

        private void Start()
        {
            var temp = GetComponentsInChildren<Anchor>();
            foreach (Anchor anchor in temp)
            {
                anchor.SetParentBlock(this);
            }

            // Setup anchors dictionary
            foreach (Anchor a in temp)
            {
                childrenAnchor.Add(a.GetSelfPath(), a);
            }
        }

        public void EnableColliders(bool enable)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = enable;
            }
        }
        
        public bool IsAnchorAvailable(Anchor anchor)
        {
            int index = ChildAnchors.FindIndex(a => a == anchor);
            
            if (index == -1) return false;
            if (ChildAnchors[index].IsOccupied) return false;
            
            return !anchor.IsOccupied;
        }

        public static bool CanBeSnapOn(Block block, Anchor anchor, bool checkCollision)
        {
            if (anchor.IsOccupied) return false;
            if (!anchor.IsAvailable) return false;
            if (anchor.PossibleBlocks.All(b => b.id != block.id)) return false; // Block not in possible blocks
            
            if (checkCollision)
            {
                Block posBlock = anchor.PossibleBlocks.Find(b => b.id == block.id);

                List<Collider> hits = new();
                foreach (Collider collider in posBlock.colliders)
                {
                    if (collider is BoxCollider boxCollider)
                    {
                        Vector3 size = boxCollider.size - Vector3.one * 0.01f;
                        var tempHits = Physics.OverlapBox(collider.transform.position, size / 2f,
                            collider.transform.rotation,
                            LayerMask.GetMask("Build"));

                        hits.AddRange(tempHits.Where(hit =>
                            hit.transform.gameObject != block.gameObject)); // Remove self
                    }
                    else if (collider is SphereCollider sphereCollider)
                    {
                        var tempHits = Physics.OverlapSphere(collider.transform.position, sphereCollider.radius,
                            LayerMask.GetMask("Build"));

                        hits.AddRange(tempHits.Where(hit =>
                            hit.transform.gameObject != block.gameObject)); // Remove self

                    }
                }

                if (hits.Count > 0)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public bool TryToSnapOn(Anchor anchor)
        {
            if (!CanBeSnapOn(this, anchor, true)) return false;
            
            // Move block
            Block posBlock = anchor.PossibleBlocks.Find(b => b.id == id);
            transform.position = posBlock.transform.position;
            transform.rotation = Quaternion.identity; // TODO: Rotation
            
            return true;
        }
        
        public bool TryToPlaceOn(Anchor anchor)
        {
            if (!CanBeSnapOn(this, anchor, true)) return false;
            
            Block posBlock = anchor.PossibleBlocks.Find(b => b.id == id);
            // Move block
            transform.position = posBlock.transform.position;
            transform.rotation = Quaternion.identity; // TODO: Rotation
            
            EnableColliders(true);
            
            UpdateAnchors();
            foreach (Anchor parent in anchors)
            {
                parent.ParentBlock.UpdateAnchors();
            }
            
            return true;
        }

        private void UpdateAnchors()
        {
            // List anchors attached to this block
            List<Collider> hits = new ();
            
            foreach (Collider selfCollider in colliders)
            {
                if (selfCollider is BoxCollider boxCollider)
                {
                    Vector3 size = boxCollider.size - Vector3.one * 0.01f;
                    hits.AddRange(Physics.OverlapBox(boxCollider.transform.position, size / 2f, boxCollider.transform.rotation,
                        LayerMask.GetMask("Anchor")));
                }
                else if (selfCollider is SphereCollider sphereCollider)
                {
                    hits.AddRange(Physics.OverlapSphere(selfCollider.transform.position, sphereCollider.radius,
                        LayerMask.GetMask("Anchor")));
                }
            }
            
            // Filter hits
            List<Anchor> foundAnchors = new ();
            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent(out Anchor a)) continue;
                if (!CanBeSnapOn(this, a, false)) continue;
                if (foundAnchors.Contains(a)) continue; // Already found

                foreach (Block possibleBlock in a.PossibleBlocks)
                {
                    if (possibleBlock.id == id && 
                        possibleBlock.transform.position == transform.position)
                    {
                        foundAnchors.Add(a);
                    }
                }
            }
            
            // Attach block
            foreach (var a in foundAnchors)
            {
                a.SetChildBlock(this);
                anchors.Add(a);
            }
        }
    }
}