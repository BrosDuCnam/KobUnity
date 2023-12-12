using System.Collections.Generic;
using Components.Building;
using Unity.Collections;
using UnityEngine;
using Utils;
using NetworkPlayer = Network.NetworkPlayer;

namespace Components
{
        public class PlayerBuilder : MonoBehaviour
    {
        [SerializeField] private List<BuildNode> dbg_blocks = new List<BuildNode>();

        [Header("Settings")] 
        [SerializeField] private Camera cam;
        [SerializeField] private float raycastDistance = 10f;
        
        [HideInInspector] public BuildNode selectedBlock;
        
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
            BuildAnchor targetAnchor = GetTargetAnchor();
            found = targetAnchor != null;
            if (found)
            {
                selectedBlock.transform.position = targetAnchor.child.transform.position;
                selectedBlock.transform.rotation = targetAnchor.child.transform.rotation;
                
                return;
            }
            
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
        
        public BuildAnchor GetTargetAnchor()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            List<BuildAnchor> anchors = new List<BuildAnchor>();
            
            var hits = Physics.RaycastAll(cam.ScreenPointToRay(Input.mousePosition), raycastDistance,
                LayerMask.GetMask("Anchor"));
            
            Debug.DrawRay(cam.ScreenPointToRay(screenCenter).origin, cam.ScreenPointToRay(Input.mousePosition).direction * raycastDistance, Color.red);
            
            foreach (var hit in hits)
            {
                if (!hit.collider.TryGetComponent(out BuildAnchor anchor)) continue;
                if (!anchor.IsAvailable) continue;
                if (anchor.child.type != selectedBlock.type) continue;
                
                if (anchor.parent.CheckPermissionToBuild(anchor.AnchorId))
                {
                    anchors.Add(anchor);
                }
            }
            
            // Order by distance
            anchors.Sort((a, b) =>
            {
                float aDistance = Vector3.Distance(a.transform.position, cam.transform.position);
                float bDistance = Vector3.Distance(b.transform.position, cam.transform.position);
                return aDistance.CompareTo(bDistance);
            });
            if (anchors.Count > 0) return anchors[0];
            
            return null;
        }

        public void Place()
        {
            if (selectedBlock == null) return;
            
            BuildAnchor targetAnchor = GetTargetAnchor();
            if (targetAnchor == null) return;

            targetAnchor.parent.build.Build(targetAnchor);


            int selectedBlockIndex = dbg_blocks.FindIndex(b => b.type == selectedBlock.type);
            Destroy(selectedBlock.gameObject);
            selectedBlock = null;
            
            SelectBlock(selectedBlockIndex);
        }
        
        
        public void SelectBlock(int index)
        {
            if (index < 0 || index >= dbg_blocks.Count) return;
            
            var type = dbg_blocks[index].type;
            if (selectedBlock != null && selectedBlock.type == type) return;
            if (selectedBlock != null) Destroy(selectedBlock.gameObject);
            
            selectedBlock = Instantiate(UResources.GetBuildNodePrefab(type), transform);
        }
    }
}