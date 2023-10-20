using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components.Building
{
    public class Block : MonoBehaviour
    {
        [field: SerializeField] public int id { get; private set; }

        [Space] 
        
        [SerializeField] private List<Anchor> anchors = new (); // fallback attach points
        
        [Header("References")]
        [SerializeField] private List<Anchor> childrenAnchor = new (); // anchor, constraints
        [SerializeField] private List<Collider> colliders = new ();
        
        public List<Anchor> ChildAnchors => childrenAnchor;
        
        public void OnValidate()
        {
            if (id == 0)
            {
                id = Guid.NewGuid().GetHashCode();
            }
        }

        private void Start()
        {
            childrenAnchor.AddRange(GetComponentsInChildren<Anchor>());
            
            foreach (Anchor anchor in ChildAnchors)
            {
                anchor.SetParentBlock(this);
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
            int index = childrenAnchor.FindIndex(a => a == anchor);
            
            if (index == -1) return false;
            if (childrenAnchor[index].IsOccupied) return false;
            
            return !anchor.IsOccupied;
        }

        public static bool CanBeSnapOn(Block block, Anchor anchor)
        {
            if (anchor.IsOccupied) return false;
            if (!anchor.IsAvailable) return false;
            if (anchor.PossibleBlocks.All(b => b.id != block.id)) return false; // Block not in possible blocks
            
            Block posBlock = anchor.PossibleBlocks.Find(b => b.id == block.id);
            
            List<Collider> hits = new ();
            foreach (Collider collider in posBlock.colliders)
            {
                if (collider is BoxCollider boxCollider)
                {
                    Vector3 size = boxCollider.size - Vector3.one * 0.01f;
                    var tempHits = Physics.OverlapBox(collider.transform.position, size / 2f, collider.transform.rotation,
                        LayerMask.GetMask("Build"));

                    hits.AddRange(tempHits.Where(hit => hit.transform.gameObject != block.gameObject)); // Remove self
                    
                    // Draw box
                    Debug.DrawRay(collider.transform.position, collider.transform.forward * size.z / 2f, Color.red);
                    Debug.DrawRay(collider.transform.position, collider.transform.forward * -size.z / 2f, Color.red);
                    Debug.DrawRay(collider.transform.position, collider.transform.right * size.x / 2f, Color.red);
                    Debug.DrawRay(collider.transform.position, collider.transform.right * -size.x / 2f, Color.red);
                    Debug.DrawRay(collider.transform.position, collider.transform.up * size.y / 2f, Color.red);
                    Debug.DrawRay(collider.transform.position, collider.transform.up * -size.y / 2f, Color.red);
                
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    var tempHits = Physics.OverlapSphere(collider.transform.position, sphereCollider.radius,
                        LayerMask.GetMask("Build"));
                    
                    hits.AddRange(tempHits.Where(hit => hit.transform.gameObject != block.gameObject)); // Remove self

                }
            }
            
            if (hits.Count > 0)
            {
                return false;
            }
            
            return true;
        }
        
        public bool TryToSnapOn(Anchor anchor)
        {
            if (!CanBeSnapOn(this, anchor)) return false;
            
            // Move block
            Block posBlock = anchor.PossibleBlocks.Find(b => b.id == id);
            transform.position = posBlock.transform.position;
            transform.rotation = posBlock.transform.rotation;
            
            return true;
        }
        
        public bool TryToPlaceOn(Anchor anchor)
        {
            if (!CanBeSnapOn(this, anchor)) return false;
            
            Block posBlock = anchor.PossibleBlocks.Find(b => b.id == id);
            // Move block
            transform.position = posBlock.transform.position;
            transform.rotation = posBlock.transform.rotation;
            
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
                if (!CanBeSnapOn(this, a)) continue;
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