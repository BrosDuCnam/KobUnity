using System.Collections.Generic;
using UnityEngine;
using Utils;
using NetworkPlayer = Network.NetworkPlayer;

namespace Components.Building
{
    public class PlayerBuilder : MonoBehaviour
    {
        [SerializeField] private List<Block> dbg_blocks = new List<Block>();

        [Header("Settings")] 
        [SerializeField] private Camera cam;
        [SerializeField] private float raycastDistance = 10f;
        
        [HideInInspector] public Block selectedBlock;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectBlock(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectBlock(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectBlock(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectBlock(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SelectBlock(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SelectBlock(5);
            
            if (selectedBlock == null) return;
            
            bool found = false;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            
            // Snap block
            Anchor targetAnchor = GetTargetAnchor();
            if (targetAnchor != null)
            {
                found = selectedBlock.TryToSnapOn(targetAnchor);
            }
            
            if (found) return;
            
            // Move block
            selectedBlock.transform.localRotation = Quaternion.identity;
            
            LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Anchor") | 1 << LayerMask.NameToLayer("Player"));
            
            var hits = Physics.RaycastAll(cam.ScreenPointToRay(screenCenter), raycastDistance, layerMask);
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject == selectedBlock.gameObject) continue;
                if (hit.transform.gameObject.GetComponentInParent(typeof(NetworkPlayer)) != null) continue;
                
                selectedBlock.transform.position = hit.point;
                found = true;
            }
            
            if (found) return;
            
            selectedBlock.transform.position = cam.transform.forward * raycastDistance;
        }
        
        public Anchor GetTargetAnchor()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            var anchors = Physics.RaycastAll(cam.ScreenPointToRay(Input.mousePosition), raycastDistance,
                LayerMask.GetMask("Anchor"));
            
            Debug.DrawRay(cam.ScreenPointToRay(screenCenter).origin, cam.ScreenPointToRay(Input.mousePosition).direction * raycastDistance, Color.red);
            
            foreach (var hit in anchors)
            {
                if (!hit.collider.TryGetComponent(out Anchor anchor)) continue;
                if (!anchor.IsAvailable) continue;
                if (selectedBlock.ChildAnchors.Contains(anchor)) continue;
                
                if (Block.CanBeSnapOn(selectedBlock, anchor))
                {
                    return anchor;
                }
            }
            
            return null;
        }
        
        
        public void SelectBlock(int index)
        {
            if (index < 0 || index >= dbg_blocks.Count) return;
            
            int id = dbg_blocks[index].id;
            if (selectedBlock != null && selectedBlock.id == id) return;
            if (selectedBlock != null) Destroy(selectedBlock.gameObject);
            
            selectedBlock = Instantiate(UResources.GetBlockById(id), transform);
            selectedBlock.EnableColliders(false);
        }
    }
}