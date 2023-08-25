using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MNetwork))]
    public class MNetworkInspector : UnityEditor.Editor
    {
        // Reference to the NetworkManager custom editor
        NetworkManagerEditor networkManagerEditor;

        // Called when the inspector GUI is being drawn
        public override void OnInspectorGUI()
        {
            // Display the custom inspector for NetworkManager
            DrawNetworkManagerInspector();
            
            // Display the custom inspector for MNetwork
            DrawMNetworkInspector();
        }

        // Draw the custom inspector for MNetwork
        private void DrawMNetworkInspector()
        {
            DrawDefaultInspector();
        }

        // Draw the custom inspector for NetworkManager
        private void DrawNetworkManagerInspector()
        {
            // If networkManagerEditor is null, create an editor for the NetworkManager type
            if (networkManagerEditor == null)
            {
                
                networkManagerEditor = (NetworkManagerEditor)CreateEditor(target, typeof(NetworkManagerEditor));
            }

            // Display the custom inspector for NetworkManager
            if (networkManagerEditor != null)
            {
                networkManagerEditor.OnInspectorGUI();
            }
        }

        // Called when the inspector is disabled
        private void OnDisable()
        {
            // Clean up the networkManagerEditor
            if (networkManagerEditor != null)
            {
                DestroyImmediate(networkManagerEditor);
                networkManagerEditor = null;
            }
        }
    }
}